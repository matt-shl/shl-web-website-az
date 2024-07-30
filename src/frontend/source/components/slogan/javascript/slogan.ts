// @ts-ignore
import InfiniteMarquee from 'infinite-marquee'

import ScreenDimensions from '@/utilities/screen-dimensions'

class Slogan {
  element: HTMLElement
  marquee: any

  constructor(element: HTMLElement) {
    this.element = element

    this.init()
  }

  init() {
    this.marquee = new InfiniteMarquee({
      el: this.element,
      direction: 'left',
      duration: ScreenDimensions.isTabletPortraitAndBigger ? 30 : 20,
      css: true,
    })
  }
}

export default Slogan
