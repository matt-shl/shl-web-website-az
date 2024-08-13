import RafThrottle from '@utilities/raf-throttle'

const JS_CLASS_FOOTER = '.c-footer'
const JS_CLASS_IS_ACTIVE = 'is--active'

class MobileFloatingButton {
  element: HTMLElement
  footer: HTMLElement | null
  elementTop: number

  constructor(element: HTMLElement) {
    this.element = element
    this.footer = document.querySelector(JS_CLASS_FOOTER)
    this.elementTop = this.element.getBoundingClientRect().top
    this.bindEvents()
  }

  bindEvents() {
    RafThrottle.set([
      {
        element: window,
        event: 'scroll',
        namespace: 'mobile-floating-button-scroll',
        fn: () => this.toggleActiveClass(),
      },
    ])
  }

  toggleActiveClass() {
    if (!this.footer) return
    const { top } = this.footer.getBoundingClientRect()
    this.element.classList[this.elementTop < top ? 'add' : 'remove'](JS_CLASS_IS_ACTIVE)
  }
}

export default MobileFloatingButton
