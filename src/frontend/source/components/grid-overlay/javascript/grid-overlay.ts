const CLASS_GRID_OVERLAY_IS_VISIBLE = 'grid-overlay--is-visible'

class GridOverlay {
  element: HTMLElement

  constructor(element: HTMLElement) {
    this.element = element
    this.#bindEvents()
  }

  #bindEvents() {
    document.addEventListener('keydown', this.#toggleOverlay.bind(this))
  }

  #toggleOverlay(event: KeyboardEvent) {
    if (event.shiftKey && event.key.toLowerCase() === 'g') {
      this.element.classList.toggle(CLASS_GRID_OVERLAY_IS_VISIBLE)
    }
  }
}

export default GridOverlay
