import Events from '@/utilities/events'
import InView from '@/utilities/in-view'

const LAZY_IMAGE_HOOK = '.c-image'
const LAZY_IMAGE_SRC_HOOK = 'data-src'
const LAZY_IMAGE_SRCSET_HOOK = 'data-srcset'
const LAZY_IMAGE_ANIMATE_IN_CLASS = 'image--is-loaded'

class LazyImage {
  images = getImageNodes<HTMLElement>(LAZY_IMAGE_HOOK)

  constructor() {
    this._bindEvents()
    this._setObserverables()
  }
  _bindEvents() {
    Events.$on<HTMLElement>('lazyimage::load', (_, element) => this._loadImage(element))
    Events.$on('lazyimage::update', () => this._updateImages())
  }
  _setObserverables() {
    InView.addElements(this.images, 'lazyimage::load')
  }
  /**
   * Load the image
   */
  _loadImage(element: HTMLElement) {
    const picture = element.querySelector('picture')
    const image = element.querySelector('img')
    if (!image) return

    if (picture) {
      image.onload = () => this._renderImage(element)
      const sources = picture.querySelectorAll('source')
      sources.forEach(source => {
        const srcset = source.dataset.srcset
        if (srcset) {
          source.srcset = srcset
          source.removeAttribute(LAZY_IMAGE_SRCSET_HOOK)
        }
      })
    } else {
      const src = image.getAttribute(LAZY_IMAGE_SRC_HOOK)
      const srcset = image.getAttribute(LAZY_IMAGE_SRCSET_HOOK)
      image.removeAttribute(LAZY_IMAGE_SRC_HOOK)
      image.removeAttribute(LAZY_IMAGE_SRCSET_HOOK)

      // If there is no data-src set just render the element.
      if (!src || (!src && image.src)) {
        this._renderImage(element)
        return
      }

      image.src = ''
      image.onload = () => this._renderImage(element)
      image.src = src
      if (srcset) image.srcset = srcset
    }
  }
  /**
   * Set image source
   */
  _renderImage(element: HTMLElement) {
    const image = element.querySelector<HTMLImageElement>('img')
    if (!image) return
    element.classList.add(LAZY_IMAGE_ANIMATE_IN_CLASS)
  }
  /**
   * Update new images
   */
  _updateImages() {
    this.images = getImageNodes(LAZY_IMAGE_HOOK)
    this._setObserverables()
  }
}

function getImageNodes<T extends HTMLElement>(selector: string) {
  return Array.from(document.querySelectorAll<T>(selector))
}

export default new LazyImage()
