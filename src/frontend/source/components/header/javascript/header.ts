import { html } from '@utilities/dom-elements'
import RafThrottle from '@utilities/raf-throttle'
import ScreenDimensions from '@utilities/screen-dimensions'

const CLASS_HEADER_IS_STICKY = 'header--is-sticky'
const CLASS_HEADER_IS_GOING_UP = 'header--is-going-up'
const CLASS_HEADER_IS_HIDDEN = 'header--is-hidden'
const HIDE_THRESHOLD = 200

class Header {
  private prevScrollValue = 0
  private threshold = 0

  constructor(_element: HTMLElement) {
    this.threshold = ScreenDimensions.isTabletPortraitAndBigger ? 32 : 16

    this.bindEvents()
    this.handlePageScroll()
  }

  bindEvents() {
    RafThrottle.set([
      {
        namespace: 'window::scroll-header',
        element: window,
        event: 'scroll',
        fn: () => this.handlePageScroll(),
      },
    ])
  }

  private handlePageScroll() {
    const scrollValue = window.pageYOffset
    const isSticky = scrollValue > this.threshold
    const isGoingUp = scrollValue < this.prevScrollValue

    html.classList[isSticky ? 'add' : 'remove'](CLASS_HEADER_IS_STICKY)
    html.classList[scrollValue > HIDE_THRESHOLD ? 'add' : 'remove'](CLASS_HEADER_IS_HIDDEN)
    html.classList[isGoingUp && isSticky ? 'add' : 'remove'](CLASS_HEADER_IS_GOING_UP)

    this.prevScrollValue = scrollValue
  }
}

export default Header
