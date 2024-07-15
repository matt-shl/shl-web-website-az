const JS_HOOK_FLYOUT_MAIN_ITEM = '[js-hook-flyout-main-item]'
const CLASS_IS_OPEN = 'is--open'

class Flyout {
  private element: HTMLElement
  private items: HTMLUListElement[]

  constructor(element: HTMLElement) {
    this.element = element;
    this.items = [...this.element.querySelectorAll(JS_HOOK_FLYOUT_MAIN_ITEM)] as HTMLUListElement[]

    this.bindEvents()
  }

  bindEvents() {
    this.items.forEach(item => {
      item.addEventListener('mouseover', () => this.openItem(item))
      item.addEventListener('keydown', event => this.handleItemKeydown(event, item))
    })
  }

  handleItemKeydown(event: KeyboardEvent, item: HTMLUListElement) {
    if (event.key === 'Enter') {
      event.preventDefault()
      this.openItem(item)
    }
  }

  openItem = (item: HTMLUListElement) => {
    this.closeAllItems();

    item.classList.add(CLASS_IS_OPEN)
  }

  closeItem = (item: HTMLUListElement) => {
    item.classList.remove(CLASS_IS_OPEN)
  }

  closeAllItems = () => {
    this.items.forEach(item => this.closeItem(item))
  }
}

export default Flyout
