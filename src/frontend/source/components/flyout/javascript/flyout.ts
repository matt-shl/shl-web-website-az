import Events from "@utilities/events";

const JS_HOOK_NAVIGATION_DESKTOP_ITEM = '[js-hook-navigation-desktop-item]'
const JS_HOOK_NAVIGATION_DESKTOP_ANCHOR = '[js-hook-navigation-desktop-anchor]'
const JS_HOOK_FLYOUT_MAIN_ITEM = '[js-hook-flyout-main-item]'
const JS_HOOK_FLYOUT_MAIN_ITEM_ANCHOR = '[js-hook-flyout-main-item-anchor]'
const JS_HOOK_FLYOUT_SUB_ITEM_ANCHOR = '[js-hook-flyout-sub-item-anchor]'
const JS_HOOK_FLYOUT_MAIN_CTA = '[js-hook-flyout-main-cta]'
const CLASS_IS_OPEN = 'is--open'

const MOUSE_OVER_OPEN_DELAY = 170; // Allows moving mouse to submenu while going over other items

class Flyout {
  private element: HTMLElement
  private items: HTMLUListElement[]
  private mainItemAnchors: HTMLAnchorElement[]
  private subItemAnchors: HTMLAnchorElement[]
  private mainCTAs: HTMLAnchorElement[]
  private openTimer: NodeJS.Timeout | null;

  constructor(element: HTMLElement) {
    this.element = element;
    this.items = [...this.element.querySelectorAll(JS_HOOK_FLYOUT_MAIN_ITEM)] as HTMLUListElement[]
    this.mainItemAnchors = [...this.element.querySelectorAll(JS_HOOK_FLYOUT_MAIN_ITEM_ANCHOR)] as HTMLAnchorElement[]
    this.subItemAnchors = [...this.element.querySelectorAll(JS_HOOK_FLYOUT_SUB_ITEM_ANCHOR)] as HTMLAnchorElement[]
    this.mainCTAs = [...this.element.querySelectorAll(JS_HOOK_FLYOUT_MAIN_CTA)] as HTMLAnchorElement[]

    this.bindEvents()
  }

  bindEvents() {
    this.items.forEach(item => {
      item.addEventListener('mouseover', () => {
        if (this.openTimer) clearTimeout(this.openTimer);
        this.openTimer = setTimeout(() => {
          this.openItem(item)
        }, MOUSE_OVER_OPEN_DELAY);
      })
      item.addEventListener('keydown', event => this.handleItemKeydown(event, item))
    })

    this.mainItemAnchors.forEach(mainItemAnchor => {
      const controls = mainItemAnchor.getAttribute('aria-controls')
      const children = document.querySelector(`#${controls}`)?.childElementCount || 0

      if(children === 0) {
        console.log("mainItemAnchor", mainItemAnchor, children)
        mainItemAnchor.addEventListener('click', () => {
          const headerCategory = mainItemAnchor.closest(JS_HOOK_NAVIGATION_DESKTOP_ITEM)?.querySelector(JS_HOOK_NAVIGATION_DESKTOP_ANCHOR)?.textContent?.trim()
          const headerSubcategory = mainItemAnchor.textContent?.trim();

          Events.$trigger('gtm::push', {
            data: {
              'event': 'header_menu',
              'header_category': headerCategory,          //e.g. What we offer
              'header_subcategory': headerSubcategory,    //e.g. Injectors
              'header_subcategory2': ""   //e.g. Platform 2
            }
          })
        })
      }

    })

    this.subItemAnchors.forEach(subItemAnchor => {
      subItemAnchor.addEventListener('click', () => {
        const headerCategory = subItemAnchor.closest(JS_HOOK_NAVIGATION_DESKTOP_ITEM)?.querySelector(JS_HOOK_NAVIGATION_DESKTOP_ANCHOR)?.textContent?.trim()
        const headerSubcategory = subItemAnchor.closest(JS_HOOK_FLYOUT_MAIN_ITEM)?.querySelector(JS_HOOK_FLYOUT_MAIN_ITEM_ANCHOR)?.textContent?.trim()
        const headerSubcategory2 = subItemAnchor.textContent?.trim();

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

    this.mainCTAs.forEach(mainCTA => {
      mainCTA.addEventListener('click', () => {
        const headerCategory = mainCTA.closest(JS_HOOK_NAVIGATION_DESKTOP_ITEM)?.querySelector(JS_HOOK_NAVIGATION_DESKTOP_ANCHOR)?.textContent?.trim()
        const headerSubcategory = mainCTA.textContent?.trim()

        Events.$trigger('gtm::push', {
          data: {
            'event': 'header_menu',
            'header_category': headerCategory,          //e.g. What we offer
            'header_subcategory': headerSubcategory,    //e.g. View all
          }
        })
      });
    })
  }

  handleItemKeydown(event: KeyboardEvent, item: HTMLUListElement) {
    if (event.key === 'Enter' || event.key === ' ') {
      this.openItem(item)
    }
  }

  openItem = (item: HTMLUListElement) => {
    this.closeAllItems();

    item.classList.add(CLASS_IS_OPEN)
    item.querySelector(JS_HOOK_FLYOUT_MAIN_ITEM_ANCHOR)?.setAttribute("aria-expanded", "true")
  }

  closeItem = (item: HTMLUListElement) => {
    item.classList.remove(CLASS_IS_OPEN)
    item.querySelector(JS_HOOK_FLYOUT_MAIN_ITEM_ANCHOR)?.setAttribute("aria-expanded", "false")
  }

  closeAllItems = () => {
    this.items.forEach(item => this.closeItem(item))
  }
}

export default Flyout
