import Events from "@utilities/events";

const BUTTON_SELECTOR = '*[class*="c-button"]:not(.button--icon-only)'

class EventDetail {
  private element: HTMLElement
  private registerBtn: HTMLAnchorElement

  constructor(element: HTMLElement) {
    this.element = element;
    this.registerBtn = this.element.querySelector(BUTTON_SELECTOR) as HTMLAnchorElement

    this.bindEvents()
  }

  bindEvents() {
    this.registerBtn.addEventListener('click', () => {
      const optionClicked = this.element.querySelector('h2')?.textContent?.trim()

      if (!optionClicked) return

      Events.$trigger('gtm::push', {
        data: {
          'event': 'register_event',
          'option_clicked': optionClicked
        }
      })
    });
  }
}

export default EventDetail
