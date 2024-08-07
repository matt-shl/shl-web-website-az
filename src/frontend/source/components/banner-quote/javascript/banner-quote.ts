const JS_HOOK_BANNER_IMAGE_CAROUSEL = '[js-hook-banner-quote-image-carousel]'
const JS_HOOK_BANNER_IMAGE = '[js-hook-banner-quote-image]'
const JS_HOOK_BANNER_TEXT_CONTAINER = '[js-hook-banner-quote-text-container]'

const JS_HOOK_BANNER_PAGINATION_CURRENT_INDEX = '[js-hook-banner-quote-pagination-current-index]'
const JS_HOOK_BANNER_PAGINATION_PREV_BUTTON = '[js-hook-banner-quote-pagination-prev-button]'
const JS_HOOK_BANNER_PAGINATION_NEXT_BUTTON = '[js-hook-banner-quote-pagination-next-button]'

class BannerQuote {
  element: HTMLElement
  imageCarousel: HTMLElement | null
  images: HTMLElement[] | null
  currentLabelElement: HTMLElement | null
  textContainerElements: HTMLElement[] | null
  prevButton: HTMLButtonElement | null
  nextButton: HTMLButtonElement | null
  totalImages: number
  currentIndex: number

  constructor(element: HTMLElement) {
    this.element = element
    this.imageCarousel = this.element.querySelector(JS_HOOK_BANNER_IMAGE_CAROUSEL)
    this.images = Array.from(this.element.querySelectorAll(JS_HOOK_BANNER_IMAGE))
    this.totalImages = this.images?.length || 0
    this.currentLabelElement = this.element.querySelector(JS_HOOK_BANNER_PAGINATION_CURRENT_INDEX)
    this.textContainerElements = Array.from(
      this.element.querySelectorAll(JS_HOOK_BANNER_TEXT_CONTAINER),
    )
    this.prevButton = this.element.querySelector(JS_HOOK_BANNER_PAGINATION_PREV_BUTTON)
    this.nextButton = this.element.querySelector(JS_HOOK_BANNER_PAGINATION_NEXT_BUTTON)
    this.currentIndex = 0

    this.#bindEvents()
  }

  #bindEvents() {
    if (this.prevButton && this.nextButton) {
      this.prevButton.addEventListener('click', () => this.#setCurrentIndex(false))
      this.nextButton.addEventListener('click', () => this.#setCurrentIndex())
    }
  }

  #setCurrentIndex(next: boolean = true) {
    this.currentIndex = next
      ? (this.currentIndex + 1) % this.totalImages
      : (this.currentIndex - 1 + this.totalImages) % this.totalImages

    this.#changeLabels()
    this.#disableButtons()
    this.#changeSlide()
    this.#setCurrentClassOnImage()
  }

  #changeLabels() {
    if (this.currentLabelElement) {
      this.currentLabelElement.textContent = String(this.currentIndex + 1)
    }
    if (this.textContainerElements) {
      this.textContainerElements.forEach((textContainer, index) => {
        textContainer.classList.toggle('is--active', index === this.currentIndex)
      })
    }
  }

  #disableButtons() {
    if (this.currentIndex === 0) {
      this.prevButton?.setAttribute('disabled', 'disabled')
      this.nextButton?.removeAttribute('disabled')
    } else if (this.currentIndex === this.totalImages - 1) {
      this.prevButton?.removeAttribute('disabled')
      this.nextButton?.setAttribute('disabled', 'disabled')
    } else {
      this.prevButton?.removeAttribute('disabled')
      this.nextButton?.removeAttribute('disabled')
    }
  }

  #changeSlide() {
    if (!this.images) return
    this.images[0].style.marginLeft = `-${this.currentIndex * 100}%`
  }

  #setCurrentClassOnImage() {
    if (!this.images) return
    this.images.forEach((image, index) => {
      image.classList.toggle('is--active', index === this.currentIndex)
    })
  }
}

export default BannerQuote
