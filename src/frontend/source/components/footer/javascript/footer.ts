import Events from "@utilities/events";

const JS_HOOK_FOOTER_SOCIAL_LINK = '[js-hook-footer-social-link]'
const JS_HOOK_FOOTER_NAVIGATION_CONTAINER = '[js-hook-footer-navigation-container]'
const JS_HOOK_FOOTER_NAVIGATION_CONTAINER_TITLE = '[js-hook-footer-navigation-container-title]'

class Footer {
  private element: HTMLElement
  private navigationLinks: HTMLAnchorElement[]
  private socialLinks: HTMLAnchorElement[]

  constructor(element: HTMLElement) {
    this.element = element;
    this.navigationLinks = [...this.element.querySelectorAll(`a:not(JS_HOOK_FOOTER_SOCIAL_LINK)`)] as HTMLAnchorElement[]
    this.socialLinks = [...this.element.querySelectorAll(JS_HOOK_FOOTER_SOCIAL_LINK)] as HTMLAnchorElement[]

    this.bindEvents()
  }

  bindEvents() {
    this.navigationLinks.forEach(navigationLink => {
      navigationLink.addEventListener('click', () => {
        const footerCategory = navigationLink.closest(JS_HOOK_FOOTER_NAVIGATION_CONTAINER)?.querySelector(JS_HOOK_FOOTER_NAVIGATION_CONTAINER_TITLE)?.textContent?.trim() || navigationLink.closest('.c-accordion')?.querySelector('.accordion__item-summary')?.textContent?.trim()
        const footerSubcategory = navigationLink.textContent?.trim()

        Events.$trigger('gtm::push', {
          data: {
            'event': 'footer',
            'footer_category': footerCategory || "",          //e.g. Explore our products
            'footer_subcategory': footerSubcategory || ""     //e.g. Autoinjectors
          }
        })
      })
    })

    this.socialLinks.forEach(socialLink => {
      socialLink.addEventListener('click', () => {
        const optionClicked = socialLink.ariaLabel
        Events.$trigger('gtm::push', {
          data: {
            'event': 'social_media',
            'option_clicked': optionClicked         //e.g. LinkedIn
          }
        })
      })
    })
  }
}

export default Footer
