import Events from "@utilities/events";

class CardKnowledge {
  private element: HTMLElement
  private type: string

  constructor(element: HTMLElement) {
    this.element = element;
    this.type = this.element.dataset.type || ""

    this.bindEvents()
  }

  bindEvents() {
    if(this.type === 'download') {
      this.element.addEventListener('click', () => {
        const optionClicked = this.element.querySelector('h3')?.textContent?.trim();

        if(!optionClicked) return
        Events.$trigger('gtm::push', {
          data: {
            'event': 'select_download',
            'option_clicked': optionClicked
          }
        })
      })
    }
  }
}

export default CardKnowledge
