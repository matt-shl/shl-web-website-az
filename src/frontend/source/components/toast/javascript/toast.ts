import Events from '@/utilities/events'

const JS_HOOK_TOAST_CLOSE_BTN = '[js-hook-toast-close-btn]'

class Toast {
  element: HTMLDialogElement | HTMLDivElement
  btnClose: HTMLButtonElement

  constructor(element: HTMLDialogElement | HTMLDivElement) {
    if (!element.id) return
    this.element = element
    this.btnClose = this.element.querySelector(JS_HOOK_TOAST_CLOSE_BTN)!

    this.#bindEvents()
  }

  handleCloseBtnClick = () => this.close()

  #bindEvents() {
    this.btnClose.addEventListener('click', this.handleCloseBtnClick)
  }

  close() {
    this.#unbindAll()

    Events.$trigger('toastManager::removeToast', {
      data: { id: this.element.id },
    })
  }

  #unbindAll() {
    this.btnClose.removeEventListener('click', this.handleCloseBtnClick)
  }
}

export default Toast
