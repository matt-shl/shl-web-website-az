class LanguageSelector {
  element: HTMLSelectElement

  constructor(element: HTMLSelectElement) {
    this.element = element
    this.bindEvents()
  }

  bindEvents() {
    this.element.addEventListener('change', () => {
      const selectedUrl = this.element.value
      if (selectedUrl) {
        window.location.href = selectedUrl
      }
    })
  }
}

export default LanguageSelector
