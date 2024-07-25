const LAP_ACTIVE = 'form__item--lap-active'
type InputType = HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
const INPUT_QUERY = 'input, textarea, select'
const FILE = 'file'
const SELECT = 'SELECT'

const JS_HOOK_MODAL = '[js-hook-modal]'

class LabelAsPlaceholder {
  element: HTMLElement
  input: InputType | null
  autofilled: boolean
  inputModalParent: HTMLElement | null

  constructor(element: HTMLElement) {
    this.element = element
    this.input = this.element.querySelector<InputType>(INPUT_QUERY)
    this.autofilled = false
    this.inputModalParent = this.element.closest(JS_HOOK_MODAL)

    if (this.element && this.input) {
      this.toggleLabelClass()
      this.bindEvents()

      if (!this.inputModalParent) {
        const clickEvent = new MouseEvent('click', {
          bubbles: true,
          cancelable: true,
          view: window,
        })

        setTimeout(() => {
          // trigger click to update field if value is set by browser autofill
          this.input?.dispatchEvent(clickEvent)
        }, 800)
      }
    }
  }

  bindEvents() {
    this.input?.addEventListener('change', () => this.toggleLabelClass())

    if (this.input?.type !== FILE && this.input?.tagName !== SELECT) {
      this.input?.addEventListener('input', () => this.toggleLabelClass(true))
      this.input?.addEventListener('focus', () => this.toggleLabelClass(true))
      this.input?.addEventListener('focusout', () => this.toggleLabelClass())
    }
  }

  toggleLabelClass(forceAnimateLabel?: boolean) {
    forceAnimateLabel = this.input?.type === 'date' ? true : forceAnimateLabel

    const action = forceAnimateLabel || this.input?.value ? 'add' : 'remove'
    this.element.classList[action](LAP_ACTIVE)
  }
}

export default LabelAsPlaceholder
