const JS_HOOK_RICH_TEXT_CONTENT = '[js-hook-rich-text-container]'
const JS_HOOK_RICH_TEXT_BUTTON = '[js-hook-rich-text-button]'
const CLASS_RICH_TEXT_CLOSED = 'c-rich-text--is-closed'
const CLASS_READ_MORE_DISABLED = 'c-rich-text--read-more-disabled'
const ATTR_READ_MORE_DISABLED = 'read-more-disabled'

class RichText {
  element: HTMLElement
  maxHeightPx: number
  content: HTMLElement | null
  contentHeight: number
  needsCollapsing: boolean
  isClosed: boolean
  readMoreDisabled: boolean
  toggleButtons: NodeListOf<HTMLButtonElement>

  constructor(element: HTMLElement) {
    this.element = element

    this.maxHeightPx = +(this.element.dataset.maxHeightPx || 200)
    this.readMoreDisabled = this.element.hasAttribute(ATTR_READ_MORE_DISABLED)
    this.content = this.element.querySelector(JS_HOOK_RICH_TEXT_CONTENT)
    this.toggleButtons = this.element.querySelectorAll(JS_HOOK_RICH_TEXT_BUTTON)
    if (this.content) this.needsCollapsing = this.content?.scrollHeight > this.maxHeightPx

    this.#init()
    this.#bindEvents()
  }

  #init() {
    if (this.readMoreDisabled || !this.needsCollapsing) {
      this.#disableReadMore()
    } else {
      this.#collapse()
    }
  }

  #bindEvents() {
    this.toggleButtons.forEach(button =>
      button?.addEventListener('click', () => {
        this.#toggleOpenState()
      }),
    )
  }

  #disableReadMore() {
    this.isClosed = false
    this.element.classList.add(CLASS_READ_MORE_DISABLED)
  }

  #collapse() {
    this.isClosed = true
    this.element.classList.add(CLASS_RICH_TEXT_CLOSED)
    if (this.content) this.content.style.maxHeight = `${this.maxHeightPx}px`
  }

  #expand() {
    this.isClosed = false
    this.element.classList.remove(CLASS_RICH_TEXT_CLOSED)
    if (this.content) this.content.style.maxHeight = `${this.content?.scrollHeight}px`
  }

  #toggleOpenState() {
    this.isClosed ? this.#expand() : this.#collapse()
  }
}

export default RichText
