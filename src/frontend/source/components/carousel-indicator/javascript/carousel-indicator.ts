import gsap from 'gsap'

const JS_HOOK_CAROUSEL_INDICATOR_ICONS = '[js-hook-carousel-indicator-icons]'
const JS_HOOK_CAROUSEL_INDICATOR_CONTAINER = '[js-hook-carousel-indicator-container]'
const JS_HOOK_CAROUSEL = '[js-hook-carousel]'

const CLASS_IS_PRESSED = 'is-pressed'

class CarouselIndicator {
  element: HTMLElement
  container: HTMLElement | null
  iconsContainer: HTMLElement | null
  isThereACarousel: boolean = false
  isPressed: boolean = false
  constructor(element: HTMLElement) {
    this.element = element
    this.container = this.element.querySelector(JS_HOOK_CAROUSEL_INDICATOR_CONTAINER)
    this.iconsContainer = this.element.querySelector(JS_HOOK_CAROUSEL_INDICATOR_ICONS)
    this.isThereACarousel = document.querySelector(JS_HOOK_CAROUSEL) !== null

    if (this.container && this.iconsContainer && this.isThereACarousel) {
      this.#initCarouselIndicator()
    } else {
      this.#destroyCarouselIndicator()
    }
  }

  #initCarouselIndicator() {
    gsap.set(this.container, { xPercent: -50, yPercent: -50 })
    gsap.set(this.iconsContainer, { xPercent: -50, yPercent: -50 })

    const xTo = gsap.quickTo(this.container, 'x', {
        duration: 0.3,
        ease: 'power3',
      }),
      yTo = gsap.quickTo(this.container, 'y', { duration: 0.3, ease: 'power3' })
    const xToIcons = gsap.quickTo(this.iconsContainer, 'x', {
      duration: 0.25,
      ease: 'power3',
    })
    const yToIcons = gsap.quickTo(this.iconsContainer, 'y', { duration: 0.25, ease: 'power3' })

    const updatePositions = (e: MouseEvent) => {
      xTo(e.clientX)
      yTo(e.clientY)

      xToIcons(e.clientX)
      yToIcons(e.clientY)
    }

    // Call updateIconPositions whenever this.element moves
    document.addEventListener('mousemove', updatePositions)

    //Define the scale animation for showing and hiding the indicator
    const scaleNormal = () =>
      gsap.to(this.container, { scale: 1, duration: 0.6, ease: 'power3.out' })
    const scaleDown = () => {
      if (this.isPressed) {
        gsap.to(this.container, { scale: 0.2, duration: 0.6, ease: 'power3.out' })
      }
    }

    document.addEventListener('mousedown', e => {
      this.isPressed = true
      this.element.classList.add(CLASS_IS_PRESSED)
      updatePositions(e)
      scaleDown()
    })

    document.addEventListener('mouseup', () => {
      this.isPressed = false
      this.element.classList.remove(CLASS_IS_PRESSED)
      scaleNormal()
    })
  }

  #destroyCarouselIndicator() {
    this.element.remove()
  }
}

export default CarouselIndicator
