import Events from '@utilities/events'
import Swiper from 'swiper'
import { A11y, Scrollbar } from 'swiper/modules'

import RafThrottle from '@/utilities/raf-throttle'
import ScreenDimensions from '@/utilities/screen-dimensions'

const JS_HOOK_CAROUSEL_SWIPE_INDICATOR = '[js-hook-carousel-indicator]'

const CLASS_CAROUSEL_PAGINATION_ELEM = 'carousel__pagination'
const CLASS_CAROUSEL_PAGINATION_ELEM_FILL = 'carousel__pagination-fill'

class Carousel {
  element: HTMLElement
  swiper: Swiper
  mobileOnly: boolean
  id: string
  initialized: boolean
  swipeIndicator: HTMLElement | null
  allowPointerDownEvent: boolean

  constructor(element: HTMLElement) {
    this.element = element
    this.mobileOnly = this.element.dataset.mobileOnly === 'true'
    this.id = this.element.id
    this.initialized = false
    this.swipeIndicator = document.querySelector(JS_HOOK_CAROUSEL_SWIPE_INDICATOR)
    // Check if the carousel has links inside the slides
    this.allowPointerDownEvent =
      [...this.element.querySelectorAll('.swiper-slide > a')].length === 0

    this.bindEvents()
    this.initCarousel()
    this.#handleMouseIndicator()
  }

  bindEvents() {
    Events.$on(`swiper[${this.id}]::indexChange`, (_, data: { index: number }) =>
      this.slideToIndex(data.index),
    )
    RafThrottle.set([
      {
        element: window,
        event: 'resize',
        namespace: this.id ? `carouselResize-${this.id}` : 'carouselResize',
        fn: () => this.initCarousel(),
        delay: 250,
      },
    ])
  }

  initCarousel() {
    if (!this.mobileOnly || (this.mobileOnly && !ScreenDimensions.isTabletPortraitAndBigger)) {
      if (!this.initialized) {
        this.swiper = new Swiper(this.element, {
          breakpoints: {
            0: {
              slidesPerView: Number(this.element.dataset.slidesMobile) || 1,
              spaceBetween: Number(this.element.dataset.spaceBetweenMobile) || 16,
              allowTouchMove: true,
            },
            480: {
              slidesPerView: Number(this.element.dataset.slidesMobile) || 1,
              spaceBetween: Number(this.element.dataset.spaceBetweenMobile) || 16,
              allowTouchMove: true,
            },
            768: {
              slidesPerView: Number(this.element.dataset.slidesTabletPortrait) || 2,
              spaceBetween: Number(this.element.dataset.spaceBetweenTabletPortrait) || 16,
            },
            1024: {
              slidesPerView: Number(this.element.dataset.slidesTabletLandscape) || 3,
              spaceBetween: Number(this.element.dataset.spaceBetweenTabletLandscape) || 16,
            },
            1240: {
              slidesPerView: Number(this.element.dataset.slidesDesktop) || 3,
              spaceBetween: Number(this.element.dataset.spaceBetweenDesktop) || 16,
            },
          },
          modules: [Scrollbar, A11y],
          scrollbar: {
            el: `.${CLASS_CAROUSEL_PAGINATION_ELEM}`,
            draggable: false,
            dragClass: CLASS_CAROUSEL_PAGINATION_ELEM_FILL,
          },

          touchStartPreventDefault: !this.allowPointerDownEvent,
          observer: true,
          observeParents: true,
          grabCursor: true,
          keyboard: {
            enabled: true,
          },
          watchOverflow: true,
          on: {
            slideChange: () => this.onSlideChange(),
          },
        })

        this.initialized = true
      }
    } else {
      if (this.initialized) {
        this.swiper.destroy(true, true)
        this.initialized = false
      }
    }
  }

  #handleMouseIndicator() {
    if (!this.swiper || !this.swipeIndicator || !!this.swiper.isLocked) return
    if (!this.allowPointerDownEvent) return

    this.element.addEventListener('mouseenter', () => {
      this.swipeIndicator?.classList.add('is-active')
    })

    this.element.addEventListener('mouseleave', () => {
      this.swipeIndicator?.classList.remove('is-active')
    })
  }

  slideToIndex(index: number, speed: number = 750) {
    if (this.swiper) {
      this.swiper.slideTo(index, speed)
    }
  }

  onSlideChange() {
    const currentIndex = this.swiper.realIndex
    // Add any additional logic you want to execute when the slide changes
    Events.$trigger(`swiper[${this.id}]::slideChange`, {
      data: {
        index: currentIndex,
      },
    })
  }
}

export default Carousel
