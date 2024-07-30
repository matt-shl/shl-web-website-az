import { gsap } from 'gsap'
import { ScrollTrigger } from 'gsap/ScrollTrigger'

gsap.registerPlugin(ScrollTrigger)

const HOOK_SLOGAN_CONTENT = '[js-hook-slogan-content]'

class MarqueeOnScroll {
  element: HTMLElement
  timeline: gsap.core.Timeline
  contentElement: HTMLElement | null

  constructor(element: HTMLElement) {
    this.element = element
    this.contentElement = this.element.querySelector(HOOK_SLOGAN_CONTENT)

    if (!this.contentElement) return

    this.#init()

    this.#bindEvents()
  }

  #init() {
    this.contentElement!.appendChild(this.contentElement!.cloneNode(true))

    this.timeline = gsap.timeline({
      scrollTrigger: {
        trigger: this.contentElement,
        start: () => 'top 100%',
        end: () => 'bottom 0%',
        scrub: 0.5,
      },
    })

    this.timeline.fromTo(
      this.contentElement,
      {
        translateX: 0,
      },
      {
        translateX: '-45%',
      },
    )
  }

  #bindEvents() {
    const resizeObserver = new ResizeObserver(() => {
      ScrollTrigger.refresh()
    })
    resizeObserver.observe(document.body)
  }
}

export default MarqueeOnScroll
