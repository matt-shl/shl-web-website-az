// import gsap from 'gsap'
import Swiper from 'swiper'
import { A11y, Scrollbar } from 'swiper/modules'

import RafThrottle from '@/utilities/raf-throttle'
import ScreenDimensions from '@/utilities/screen-dimensions'

const JS_HOOK_CAROUSEL_SWIPE_INDICATOR = '[js-hook-carousel-swipe-indicator]'

const CLASS_CAROUSEL_PAGINATION_ELEM = 'carousel__pagination'
const CLASS_CAROUSEL_PAGINATION_ELEM_FILL = 'carousel__pagination-fill'

class Carousel {
  element: HTMLElement
  swiper: Swiper
  mobileOnly: boolean
  id: string
  initialized: boolean
  swipeIndicator: HTMLElement | null

  constructor(element: HTMLElement) {
    this.element = element
    this.mobileOnly = this.element.dataset.mobileOnly === 'true'
    this.id = this.element.id
    this.initialized = false
    this.swipeIndicator = this.element.querySelector(JS_HOOK_CAROUSEL_SWIPE_INDICATOR)

    this.bindEvents()
    this.initCarousel()
  }

  bindEvents() {
    RafThrottle.set([
      {
        element: window,
        event: 'resize',
        namespace: this.id ? `carouselResize-${this.id}` : 'carouselResize',
        fn: () => this.initCarousel(),
        delay: 250,
      },
    ])

    // this.handleMouseIndicator()
  }

  initCarousel() {
    if (!this.mobileOnly || (this.mobileOnly && !ScreenDimensions.isTabletPortraitAndBigger)) {
      if (!this.initialized) {
        this.swiper = new Swiper(this.element, {
          breakpoints: {
            0: {
              slidesPerView: Number(this.element.dataset.slidesMobile) || 1,
              spaceBetween: Number(this.element.dataset.spaceBetweenMobile) || 8,
              allowTouchMove: true,
            },
            480: {
              slidesPerView: Number(this.element.dataset.slidesMobile) || 1,
              spaceBetween: Number(this.element.dataset.spaceBetweenMobile) || 8,
              allowTouchMove: true,
            },
            768: {
              slidesPerView: Number(this.element.dataset.slidesTabletPortrait) || 2,
              spaceBetween: Number(this.element.dataset.spaceBetweenTabletPortrait) || 8,
            },
            1024: {
              slidesPerView: Number(this.element.dataset.slidesTabletLandscape) || 3,
              spaceBetween: Number(this.element.dataset.spaceBetweenTabletLandscape) || 8,
            },
            1240: {
              slidesPerView: Number(this.element.dataset.slidesDesktop) || 3,
              spaceBetween: Number(this.element.dataset.spaceBetweenDesktop) || 8,
            },
          },
          modules: [Scrollbar, A11y],
          scrollbar: {
            el: `.${CLASS_CAROUSEL_PAGINATION_ELEM}`,
            draggable: false,
            dragClass: CLASS_CAROUSEL_PAGINATION_ELEM_FILL,
          },
          observer: true,
          observeParents: true,
          grabCursor: true,
          keyboard: {
            enabled: true,
          },
          watchOverflow: true,
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

  // handleMouseIndicator() {
  //   gsap.set(JS_HOOK_CAROUSEL_SWIPE_INDICATOR, { xPercent: -50, yPercent: -50 })

  //   const xTo = gsap.quickTo(JS_HOOK_CAROUSEL_SWIPE_INDICATOR, 'x', {
  //       duration: 0.1,
  //       ease: 'power3',
  //     }),
  //     yTo = gsap.quickTo(JS_HOOK_CAROUSEL_SWIPE_INDICATOR, 'y', { duration: 0.1, ease: 'power3' })

  //   //Define the scale animation for showing and hiding the indicator
  //   const scaleNormal = () =>
  //     gsap.to(JS_HOOK_CAROUSEL_SWIPE_INDICATOR, { scale: 1, duration: 0.2, ease: 'power3.out' })
  //   const scaleDown = () =>
  //     gsap.to(JS_HOOK_CAROUSEL_SWIPE_INDICATOR, { scale: 0.2, duration: 0.2, ease: 'power3.out' })
  //   const scaleUp = () =>
  //     gsap.to(JS_HOOK_CAROUSEL_SWIPE_INDICATOR, { scale: 2, duration: 0.2, ease: 'power3.out' })

  //   this.element.addEventListener('mouseenter', () => {
  //     const isScrollbarLocked = this.swiper.isLocked
  //     if (isScrollbarLocked) return
  //     this.swipeIndicator?.classList.add('is-active')
  //     // Scale up when the mouse enters
  //   })

  //   this.element.addEventListener('mousedown', e => {
  //     console.log('mousedown')
  //     xTo(e.clientX)
  //     yTo(e.clientY)
  //     scaleUp() // Scale up when the mouse is down
  //   })

  //   this.element.addEventListener('mouseup', () => {
  //     console.log('mouseup')
  //     scaleNormal() // Scale up when the mouse is up
  //   })

  //   this.element.addEventListener('mousemove', e => {
  //     xTo(e.clientX)
  //     yTo(e.clientY)
  //   })

  //   this.element.addEventListener('mouseleave', () => {
  //     this.swipeIndicator?.classList.remove('is-active')
  //     // Scale down when the mouse leaves
  //   })
  //}
}

export default Carousel
