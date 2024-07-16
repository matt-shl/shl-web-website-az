const JS_HOOK_FLYOUT_MAIN_ITEM = '[js-hook-flyout-main-item]'
const JS_HOOK_FLYOUT_MAIN_ITEM_ANCHOR = '[js-hook-flyout-main-item-anchor]'
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
    if (event.key === 'Enter' || event.key === ' ') {
      event.preventDefault()
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
