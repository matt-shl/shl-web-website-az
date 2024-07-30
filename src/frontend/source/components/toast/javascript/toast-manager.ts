import Events from '@/utilities/events'
import { viewTransition } from '@/utilities/view-transition'

import Toast from './toast'

const JS_HOOK_TOAST_CLEAR_ALL = '[js-hook-toast-clear-all]'

type ToastEntry = {
  id: string
  el: HTMLElement
  instance: Toast
}

type ToastEntries = {
  [key: ToastEntry['id']]: ToastEntry
}

export type ToastProps = {
  title: string
  body?: string
  status: string
}

class ToastManager {
  element: HTMLElement
  store: ToastEntries = {}
  clearAllBtn: HTMLButtonElement | null
  popoverSupported: boolean

  constructor(element: HTMLElement) {
    this.element = element
    this.store = {}
    this.clearAllBtn = this.element.querySelector(JS_HOOK_TOAST_CLEAR_ALL)
    this.popoverSupported = HTMLElement.prototype.hasOwnProperty('popover')

    this.#bindEvents()
  }

  #bindEvents() {
    Events.$on<ToastProps>('toastManager::add', (_, data) => {
      this.#addToast(data)
    })

    Events.$on('toastManager::removeToast', (_, data) => {
      this.#removeToast(data)
    })

    this.clearAllBtn?.addEventListener('click', () => this.#clearAll())

    Events.$on<ToastProps>('toastManager::addDemoToast', () => {
      this.#addDemoToast()
    })
  }

  #clearAll() {
    Object.keys(this.store).forEach(key => {
      this.store[key].instance.close()
    })
  }

  #removeToast(data) {
    const toast = this.store[data.id]

    if (toast) {
      viewTransition(() => {
        toast.el.remove()
        this.#setPosition()
      })
    }

    delete this.store[data.id]
  }

  #addToast(data: ToastProps) {
    const toastId = new Date().getTime().toString(36)
    const elem: HTMLDialogElement | HTMLDivElement = this.#createToastElement(data, toastId)

    const toastEntry: ToastEntry = {
      id: toastId,
      el: elem,
      instance: new Toast(elem),
    }

    this.store[toastId] = toastEntry

    this.element.appendChild(elem)

    viewTransition(() => {
      this.#setPosition()

      if (this.popoverSupported) {
        toastEntry.el.showPopover()
      } else {
        toastEntry.el.showModal()
      }
    })
  }

  #createToastElement(data, toastId) {
    const { title, body, status } = data

    const toast = Object.assign(document.createElement(this.popoverSupported ? 'div' : 'dialog'), {
      innerHTML: this.#toastHTML(title, body),
      id: toastId,
      className: `toast__item toast__item--${status}`,
    })

    if (this.popoverSupported) toast.setAttribute('popover', '')

    toast.style.viewTransitionName = `${toastId}`

    return toast
  }

  #toastHTML(title: string, body: string) {
    return `
      <span class="toast__item-title">${title}</span>
      ${body ? `<span class="toast__item-body">${body}</span>` : ''}
      <button class="toast__item-close" aria-label="Close" js-hook-toast-close-btn>&times;</button>`
  }

  #setPosition() {
    let bottom = 8

    Object.keys(this.store).forEach(key => {
      this.store[key].el.style.bottom = `${bottom}px`
      bottom = bottom + this.store[key].el.clientHeight + 8
    })

    if (this.clearAllBtn) {
      this.clearAllBtn!.style.bottom = `${bottom}px`
    }
  }

  #addDemoToast() {
    Events.$trigger('toastManager::add', {
      data: {
        title: 'Toast title',
        body: 'Toast description',
        status: ['success', 'error', 'warning'][Math.floor(Math.random() * 3)],
      },
    })
  }
}

export default ToastManager
