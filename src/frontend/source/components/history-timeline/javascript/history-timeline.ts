import Events from '@utilities/events'

const JS_HOOK_ODO_METER = '[js-hook-odometer]'
const JS_HOOK_HISTORY_TIMELINE_PAGINATION_PREV_BUTTON =
  '[js-hook-history-timeline-pagination-prev-button]'
const JS_HOOK_HISTORY_TIMELINE_PAGINATION_NEXT_BUTTON =
  '[js-hook-history-timeline-pagination-next-button]'

const JS_HOOK_HISTORY_TIMELINE_IMAGE_CAROUSEL = '[js-hook-history-timeline-image-carousel]'
const JS_HOOK_HISTORY_TIMELINE_IMAGE = '[js-hook-history-timeline-image]'
const JS_HOOK_HISTORY_TIMELINE_CONTAINER = '[js-hook-history-timeline-text-container]'
const JS_HOOK_HISTORY_TIMELINE_RESTART_BUTTON = '[js-hook-history-timeline-restart-timeline-button]'

const CSS_ACTIVE_CLASS = 'is--active'

class HistoryTimeline {
  element: HTMLElement
  years: number[]
  prevButton: HTMLButtonElement | null
  nextButton: HTMLButtonElement | null
  restartButton: HTMLButtonElement | null
  imageCarousel: HTMLElement | null
  images: HTMLElement[] | null
  textContainerElements: HTMLElement[] | null
  totalItems: number
  currentIndex: number
  odometerId: string | null
  constructor(element: HTMLElement) {
    this.element = element
    this.prevButton = this.element.querySelector(JS_HOOK_HISTORY_TIMELINE_PAGINATION_PREV_BUTTON)
    this.nextButton = this.element.querySelector(JS_HOOK_HISTORY_TIMELINE_PAGINATION_NEXT_BUTTON)
    this.restartButton = this.element.querySelector(JS_HOOK_HISTORY_TIMELINE_RESTART_BUTTON)
    this.currentIndex = 0
    this.odometerId = this.element.querySelector(JS_HOOK_ODO_METER)?.id || null

    //image carousel
    this.imageCarousel = this.element.querySelector(JS_HOOK_HISTORY_TIMELINE_IMAGE_CAROUSEL)
    this.images = Array.from(this.element.querySelectorAll(JS_HOOK_HISTORY_TIMELINE_IMAGE))

    //text
    this.textContainerElements = Array.from(
      this.element.querySelectorAll(JS_HOOK_HISTORY_TIMELINE_CONTAINER),
    )

    this.years = this.textContainerElements.map(textContainer => {
      return parseInt(textContainer.dataset.year || '0', 10)
    })

    this.totalItems = this.images.length

    this.#bindEvents()
  }

  #bindEvents() {
    Events.$on(`swiper[history-timeline-carousel]::slideChange`, (_, data: { index: number }) => {
      this.currentIndex = data.index
      this.#handleHistoryChangeSideEffects()
    })

    if (this.prevButton && this.nextButton) {
      this.prevButton.addEventListener('click', () => this.#handleHistoryChange(false))
      this.nextButton.addEventListener('click', () => this.#handleHistoryChange())
    }

    this.restartButton?.addEventListener('click', () => this.#handleHistoryChange(true, 0))
  }

  #handleHistoryChange(next: boolean = true, newIndex?: number) {
    if (newIndex != null) {
      this.currentIndex = newIndex
    } else {
      this.currentIndex =
        newIndex || next
          ? (this.currentIndex + 1) % this.totalItems
          : (this.currentIndex - 1 + this.totalItems) % this.totalItems
    }

    Events.$trigger(`swiper[history-timeline-carousel]::indexChange`, {
      data: {
        index: this.currentIndex,
      },
    })

    this.#handleHistoryChangeSideEffects()
  }

  #handleHistoryChangeSideEffects() {
    if (this.odometerId) {
      Events.$trigger(`odometer[${this.odometerId}]::setNumber`, {
        data: {
          number: this.years[this.currentIndex],
        },
      })
    }

    this.#disableButtons()
    this.#changeSlide()
    this.#changeLabels()
  }

  #disableButtons() {
    if (this.currentIndex === 0) {
      this.prevButton?.setAttribute('disabled', 'disabled')
      this.nextButton?.removeAttribute('disabled')
    } else if (this.currentIndex === this.totalItems - 1) {
      this.prevButton?.removeAttribute('disabled')
      this.nextButton?.setAttribute('disabled', 'disabled')
    } else {
      this.prevButton?.removeAttribute('disabled')
      this.nextButton?.removeAttribute('disabled')
    }
  }

  #changeSlide() {
    if (!this.images || !this.imageCarousel) return
    this.imageCarousel.style.transform = `translateX(-${this.currentIndex * 100}%)`

    this.images.forEach((image, index) => {
      image.classList.toggle(CSS_ACTIVE_CLASS, index === this.currentIndex)
    })
  }

  #changeLabels() {
    if (!this.textContainerElements) return
    this.textContainerElements.forEach((textContainer, index) => {
      textContainer.classList.toggle(CSS_ACTIVE_CLASS, index === this.currentIndex)
    })
  }
}

export default HistoryTimeline
