import RafThrottle from "@utilities/raf-throttle"
import ScreenDimensions from "@utilities/screen-dimensions"

const JS_HOOK_STACKING_CARD = '[js-hook-stacking-cards-card]'
const JS_HOOK_TITLE = '[js-hook-stacking-cards-title]'
const JS_HOOK_CONTENT = '[js-hook-stacking-cards-content]'
const JS_HOOK_DESCRIPTION = '[js-hook-stacking-cards-description]'

const TITLE_MARGIN = 80

class StackingCards {
  element: HTMLElement
  cards: HTMLDivElement[]
  title: HTMLHeadElement | null
  content: HTMLDivElement | null
  description: HTMLParagraphElement | null
  titleOffset: number

  constructor(element: HTMLElement) {
    this.element = element
    this.cards = [...this.element.querySelectorAll<HTMLDivElement>(JS_HOOK_STACKING_CARD)]
    this.title = this.element.querySelector<HTMLHeadElement>(JS_HOOK_TITLE)
    this.content = this.element.querySelector<HTMLDivElement>(JS_HOOK_CONTENT)
    this.description = this.element.querySelector<HTMLParagraphElement>(JS_HOOK_DESCRIPTION)

    this.bindEvents()
  }

  bindEvents() {
    RafThrottle.set([
      {
        element: window,
        event: 'resize',
        namespace: 'stacking-cards-resize',
        fn: () => this.setStyles(),
        delay: 100
      },
    ])
    this.setStyles()

    RafThrottle.set([
      {
        element: window,
        event: 'scroll',
        namespace: 'stacking-cards-scroll',
        fn: () => this.toggleStickyClasses(),
      },
    ])
    this.toggleStickyClasses()
  }

  setStyles() {
    if (!ScreenDimensions.isTabletLandscapeAndBigger) {
      if (!this.element.getAttribute('style')) return

      // Reset styles for mobile
      this.element.removeAttribute('style')
      this.content?.removeAttribute('style')
      this.description?.removeAttribute('style')
      this.cards.forEach((card) => card.removeAttribute('style'))
      return
    }

    // Set styles for desktop
    const titleHeight = this.title?.getBoundingClientRect().height || 0
    const lastCardHeight = this.cards[this.cards.length - 1].getBoundingClientRect().height
    this.titleOffset = titleHeight + TITLE_MARGIN

    this.element.style.paddingTop = `${this.titleOffset}px`
    if (this.content) this.content.style.top = `${this.titleOffset}px`
    if (this.description) this.description.style.minHeight = `${lastCardHeight}px`
    this.cards.forEach((card) => card.style.top = `${this.titleOffset}px`)
  }

  toggleStickyClasses() {
    if (!ScreenDimensions.isTabletLandscapeAndBigger) return

    this.cards.forEach((card) => {
      const { top } = card.getBoundingClientRect()
      card.classList[top <= ((this.titleOffset)) ? 'add' : 'remove']('is--hidden')
      const secondLastCardTop = this.cards[this.cards.length - 2].getBoundingClientRect().top
      this.cards[this.cards.length - 2].classList[secondLastCardTop < this.titleOffset ? 'add' : 'remove']('is--forcibly-hidden')
    })

    const lastBehindCardIndex = [...this.element.querySelectorAll('.is--hidden')].length - 1
    this.cards.forEach((card, index) => {
      card.classList[lastBehindCardIndex === index ? 'add' : 'remove']('is--last-hidden')
    })
  }
}

export default StackingCards
