import {html} from '@utilities/dom-elements'
import Events from "@utilities/events";
import RafThrottle from '@utilities/raf-throttle'
import ScreenDimensions from '@utilities/screen-dimensions'
import header from "@components/header";

const JS_HOOK_HEADER_HAMBURGER = '[js-hook-header-hamburger]'
const CLASS_MOBILE_NAVIGATION_IS_OPEN = 'is--mobile-navigation-open'

const CLASS_HEADER_IS_STICKY = 'header--is-sticky'
const CLASS_HEADER_IS_GOING_UP = 'header--is-going-up'
const CLASS_HEADER_IS_HIDDEN = 'header--is-hidden'
const CLASS_HAS_OPEN_FLYOUT = 'has--open-flyout'
const HIDE_THRESHOLD = 200

const JS_HOOK_MOBILE_NAV_MAIN_ITEM_ANCHOR = '[js-hook-mobile-nav-main-item-anchor]'
const JS_HOOK_MOBILE_NAV_ITEM_ANCHOR = '[js-hook-mobile-nav-sub-item-anchor]'
const JS_HOOK_MOBILE_NAV_TITLE = '[js-hook-mobile-nav-title]'
const JS_HOOK_MODAL_BODY = '[js-hook-modal-body]'

class Header {
  private prevScrollValue = 0
  private threshold = 0
  private hamburger: HTMLButtonElement | null;
  private mobileNavMainItemAnchors: HTMLAnchorElement[]
  private mobileNavSubItemAnchors: HTMLAnchorElement[]

  constructor(_element: HTMLElement) {
    this.threshold = ScreenDimensions.isTabletPortraitAndBigger ? 32 : 16
    this.hamburger = document.querySelector<HTMLButtonElement>(JS_HOOK_HEADER_HAMBURGER)

    this.mobileNavMainItemAnchors = [...document.querySelectorAll(JS_HOOK_MOBILE_NAV_MAIN_ITEM_ANCHOR)] as HTMLAnchorElement[]
    this.mobileNavSubItemAnchors = [...document.querySelectorAll(JS_HOOK_MOBILE_NAV_ITEM_ANCHOR)] as HTMLAnchorElement[]

    this.bindEvents()
    this.handlePageScroll()
  }

  bindEvents() {
    Events.$on('modal::close', () => this.toggleMobileNavigationVisibility(false))
    this.hamburger?.addEventListener('click', () => this.toggleMobileNavigationVisibility(true));

    RafThrottle.set([
      {
        namespace: 'window::scroll-header',
        element: window,
        event: 'scroll',
        fn: () => this.handlePageScroll(),
      },
    ])

    this.mobileNavMainItemAnchors.forEach(mobileNavMainItemAnchor => {
      mobileNavMainItemAnchor.addEventListener('click', () => {
        const headerCategory = mobileNavMainItemAnchor.textContent?.trim()
        Events.$trigger('gtm::push', {
          data: {
            'event': 'header_menu',
            'header_category': headerCategory,          //e.g. What we offer
          }
        })
      })
    })

    this.mobileNavSubItemAnchors.forEach(mobileNavSubItemAnchor => {
      mobileNavSubItemAnchor.addEventListener('click', () => {
        const modalId = mobileNavSubItemAnchor.closest(JS_HOOK_MODAL_BODY)?.querySelector(JS_HOOK_MOBILE_NAV_TITLE)?.getAttribute('aria-controls');
        const headerCategory = document.querySelector(`#${modalId}`)?.querySelector(JS_HOOK_MOBILE_NAV_TITLE)?.textContent?.trim()
        const headerSubcategory = mobileNavSubItemAnchor.closest(JS_HOOK_MODAL_BODY)?.querySelector(JS_HOOK_MOBILE_NAV_TITLE)?.textContent?.trim()
        const headerSubcategory2 = mobileNavSubItemAnchor.textContent?.trim()
        
        Events.$trigger('gtm::push', {
          data: {
            'event': 'header_menu',
            'header_category': headerCategory,          //e.g. What we offer
            'header_subcategory': headerSubcategory,    //e.g. Injectors
            'header_subcategory2': headerSubcategory2   //e.g. Platform 2
          }
        })
      })
    })
  }

  toggleMobileNavigationVisibility(isOpen: boolean) {
    setTimeout(() => {
      html.classList[isOpen ? 'add' : 'remove'](CLASS_MOBILE_NAVIGATION_IS_OPEN)
    }, isOpen ? 400 : 0)
  }

  private handlePageScroll() {
    if (html.classList.contains(CLASS_HAS_OPEN_FLYOUT)) return

    const scrollValue = window.pageYOffset
    const isSticky = scrollValue > this.threshold
    const isGoingUp = scrollValue < this.prevScrollValue

    html.classList[isSticky ? 'add' : 'remove'](CLASS_HEADER_IS_STICKY)
    html.classList[scrollValue > HIDE_THRESHOLD ? 'add' : 'remove'](CLASS_HEADER_IS_HIDDEN)
    html.classList[isGoingUp && isSticky ? 'add' : 'remove'](CLASS_HEADER_IS_GOING_UP)

    this.prevScrollValue = scrollValue
  }
}

export default Header
