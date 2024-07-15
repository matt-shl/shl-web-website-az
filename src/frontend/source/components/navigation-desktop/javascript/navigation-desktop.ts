const JS_HOOK_ITEM = '[js-hook-navigation-desktop-item]'
const JS_HOOK_ITEM_ANCHOR = '[js-hook-navigation-desktop-anchor]'
const JS_HOOK_FLOUT = '[js-hook-flyout]'
const CLASS_IS_OPEN = 'is--open'

class NavigationDesktop {
  element: HTMLElement
  anchors: HTMLAnchorElement[]
  items: HTMLUListElement[]

  constructor(element: HTMLElement) {
    this.element = element
    this.items = [...this.element.querySelectorAll(JS_HOOK_ITEM)] as HTMLUListElement[]
    this.anchors = [...this.element.querySelectorAll(JS_HOOK_ITEM_ANCHOR)] as HTMLAnchorElement[]

    this.bindEvents()
  }

  bindEvents() {
    document.documentElement.addEventListener('keydown', event => this.handleDocumentKeydown(event))

    this.items.forEach(item => {
      item.addEventListener('mouseover', () => this.openItem(item))
      item.addEventListener('mouseout', () => this.closeItem(item))
      item.addEventListener('keydown', event => this.handleItemKeydown(event, item))
    })

    this.anchors.forEach(anchor => {
      anchor.addEventListener('focus', () => this.closeAllItems())
    })
  }

  handleItemKeydown(event: KeyboardEvent, item: HTMLUListElement) {
    if (event.key === 'Enter') {
      event.preventDefault()
      this.openItem(item)
    }
  }

  handleDocumentKeydown(event: KeyboardEvent) {
    if (event.key === 'Escape') this.closeAllItems()
  }

  openItem = (item: HTMLUListElement) => {
    item.classList.add(CLASS_IS_OPEN)

    this.setTabIndexOfAnchors(item, false)
  }

  closeItem = (item: HTMLUListElement) => {
    // item.classList.remove(CLASS_IS_OPEN)

    // this.setTabIndexOfAnchors(item, true)
  }

  setTabIndexOfAnchors = (item: HTMLUListElement, hideItem: boolean) => {
    const anchors = [...item.querySelectorAll(`${JS_HOOK_FLOUT} a`)] as HTMLAnchorElement[]
    anchors.forEach(anchor => {
      anchor.tabIndex = hideItem ? -1 : 0
    })
  }

  closeAllItems = () => {
    this.items.forEach(item => this.closeItem(item))
  }
}

export default NavigationDesktop
