const HOOK_PW_INPUT = '[js-hook-password-input]'
const HOOK_PW_TOGGLE = '[js-hook-password-toggle-btn]'

class Password {
  element: HTMLElement
  input: HTMLInputElement | null
  toggleBtn: HTMLButtonElement | null

  constructor(element: HTMLElement) {
    this.element = element
    this.input = this.element.querySelector(HOOK_PW_INPUT)
    this.toggleBtn = this.element.querySelector(HOOK_PW_TOGGLE)

    if (this.input && this.toggleBtn) {
      this.bindEvents()
    }
  }

  bindEvents() {
    this.toggleBtn!.addEventListener('click', () => {
      this.input!.type = this.input!.type === 'password' ? 'text' : 'password'
    })

    this.input!.addEventListener('input', () => {
      if (this.input!.value === '') {
        this.toggleBtn!.setAttribute('disabled', '')
      } else {
        this.toggleBtn!.removeAttribute('disabled')
      }
    })
  }
}

export default Password
