import {html} from '@utilities/dom-elements'
import Events from "@utilities/events";
import RafThrottle from '@utilities/raf-throttle'
import ScreenDimensions from '@utilities/screen-dimensions'

const JS_HOOK_HEADER_HAMBURGER = '[js-hook-header-hamburger]'
const CLASS_MOBILE_NAVIGATION_IS_OPEN = 'is--mobile-navigation-open'

const CLASS_HEADER_IS_STICKY = 'header--is-sticky'
const CLASS_HEADER_IS_GOING_UP = 'header--is-going-up'
const CLASS_HEADER_IS_HIDDEN = 'header--is-hidden'
const CLASS_HAS_OPEN_FLYOUT = 'has--open-flyout'
const HIDE_THRESHOLD = 200

class Header {
  private prevScrollValue = 0
  private threshold = 0
  private hamburger: HTMLButtonElement | null;

  constructor(_element: HTMLElement) {
    this.threshold = ScreenDimensions.isTabletPortraitAndBigger ? 32 : 16
    this.hamburger = document.querySelector<HTMLButtonElement>(JS_HOOK_HEADER_HAMBURGER)

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
  }

  toggleMobileNavigationVisibility(isOpen: boolean) {
    setTimeout(() => {
      html.classList[isOpen ? 'add' : 'remove'](CLASS_MOBILE_NAVIGATION_IS_OPEN)
    }, isOpen ? 400: 0)
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
