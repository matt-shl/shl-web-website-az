import {html} from "@utilities/dom-elements";

const JS_HOOK_ITEM = '[js-hook-navigation-desktop-item]'
const JS_HOOK_ITEM_ANCHOR = '[js-hook-navigation-desktop-anchor]'
const JS_HOOK_FLOUT = '[js-hook-flyout]'
const SELECTOR_FLYOUT_CHILDREN = `${JS_HOOK_FLOUT} a, ${JS_HOOK_FLOUT} button`
const CLASS_IS_OPEN = 'is--open'
const CLASS_HAS_OPEN_FLYOUT = 'has--open-flyout'
const CLASS_IS_HEADER_WHITE = 'is--header-white'

class NavigationDesktop {
  private element: HTMLElement
  private isHeaderWhiteOnLoad: boolean
  private anchors: HTMLAnchorElement[]
  private items: HTMLUListElement[]

  constructor(element: HTMLElement) {
    this.element = element
    this.isHeaderWhiteOnLoad = html.classList.contains(CLASS_IS_HEADER_WHITE)
    this.items = [...this.element.querySelectorAll(JS_HOOK_ITEM)] as HTMLUListElement[]
    this.anchors = [...this.element.querySelectorAll(JS_HOOK_ITEM_ANCHOR)] as HTMLAnchorElement[]

    this.bindEvents()
  }

  bindEvents() {
    document.documentElement.addEventListener('keydown', event => this.handleDocumentKeydown(event))

    this.items.forEach(item => {
      item.addEventListener('mouseover', () => this.toggleItem(item))
      item.addEventListener('mouseout', () => this.toggleItem(item, false))
      item.addEventListener('keydown', event => this.handleItemKeydown(event, item))
    })

    this.anchors.forEach(anchor => {
      anchor.addEventListener('click', event => this.handleAnchorClick(event, anchor))
      anchor.addEventListener('focus', () => this.closeAllItems())
    })
  }

  handleAnchorClick(event: MouseEvent, anchor: HTMLAnchorElement) {
    event.preventDefault();
    const item = anchor.closest(JS_HOOK_ITEM) as HTMLUListElement;
    this.toggleItem(item)
  }

  handleItemKeydown(event: KeyboardEvent, item: HTMLUListElement) {
    if (event.key === 'Enter') {
      event.preventDefault()
      this.toggleItem(item)
    }
  }

  handleDocumentKeydown(event: KeyboardEvent) {
    if (event.key === 'Escape') this.closeAllItems()
  }

  toggleItem = (item: HTMLUListElement, shouldOpen = true) => {
    const itemHasFlyout = item.querySelector(JS_HOOK_FLOUT)
    const itemIsOpen = item.classList.contains(CLASS_IS_OPEN)
    if (!itemHasFlyout || shouldOpen && itemIsOpen || !shouldOpen && !itemIsOpen) return

    item.classList[shouldOpen ? 'add' : 'remove'](CLASS_IS_OPEN)
    html.classList[shouldOpen ? 'add' : 'remove'](CLASS_HAS_OPEN_FLYOUT)
    html.classList[shouldOpen || !shouldOpen && this.isHeaderWhiteOnLoad ? 'add' : 'remove'](CLASS_IS_HEADER_WHITE)

    this.setTabIndexOfChildren(item, shouldOpen)
  }

  setTabIndexOfChildren = (item: HTMLUListElement, enableItem: boolean) => {
    const anchors = [...item.querySelectorAll(SELECTOR_FLYOUT_CHILDREN)] as HTMLAnchorElement[]
    anchors.forEach(anchor => {
      anchor.tabIndex = enableItem ? 0 : 1
    })
  }

  closeAllItems = () => {
    this.items.forEach(item => this.toggleItem(item, false))
  }
}

export default NavigationDesktop
