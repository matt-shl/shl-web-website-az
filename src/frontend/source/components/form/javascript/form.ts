import {AxiosError, Method} from 'axios'

import API from '@/utilities/api'
import Events from '@/utilities/events'
import serializeObject from '@/utilities/serialize-object'
import {rules, ValidationRule} from '@/utilities/validation'

type FormErrorMessages = Record<string, string>
type InputType = HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
const INPUT_TYPES = 'input[name]:not([type="hidden"]), textarea[name], select[name]'
type SupportedMethod = Extract<Method, 'post' | 'put' | 'get' | 'delete'>
const SUPPORTED_METHODS: SupportedMethod[] = ['post', 'put', 'get', 'delete']

const JS_HOOK_DEFAULT_SUBMIT = '[type="submit"]:not(.u-sr-only)'

const HIDDEN_CLASS = 'u-hidden'
const FORM_ITEM_CLASS = '.form__item'
const FORM_ITEM_ERROR_CLASS = 'form__item--error'
const FORM_ITEM_ERROR_ARIA = 'aria-invalid'
const FORM_ITEM_DESCRIBE_ARIA = 'aria-describedby'
const FORM_ITEM_SUCCESS_CLASS = 'form__item--success'
const FORM_ITEM_ERROR_MESSAGE_HOOK = '.form__item-error'

const DEFAULT_METHOD = 'post'

class Form {
  form: HTMLFormElement
  method: SupportedMethod
  action: HTMLFormElement['action']
  async: boolean
  events: string[]
  rules: typeof rules
  showLoader: boolean

  // DOM Elements
  inputTypes = INPUT_TYPES

  // Methods
  afterSubmitFormSuccess: ((data: any) => void) | null = null
  afterSubmitFormError: ((data: any) => void) | null = null
  beforeSubmitFormDefaultAction: ((data: FormData) => void) | null = null

  constructor(element: HTMLFormElement) {
    this.form = element
    // Config
    this.async = !!element.dataset.async
    this.method = this.getFormMethod()
    this.events = element.dataset.events ? element.dataset.events.split(',') : ['change', 'blur']
    this.rules = rules
    this.action = element.getAttribute('action') || window.location.href
    this.showLoader = !!element.dataset.loader

    // Events
    this.bindChangeEvents()
    this.bindSubmitEvents()
  }

  get inputs() {
    return [...this.form.querySelectorAll<InputType>(this.inputTypes)].filter(node => {
      if (!node.disabled) {
        const parentDisabled = !!node.closest('[disabled]')
        return !parentDisabled
      }
      return false
    })
  }

  get defaultSubmit() {
    return this.form.querySelector(JS_HOOK_DEFAULT_SUBMIT)
  }

  scrollToItem(target: HTMLElement) {
    Events.$trigger('scroll-to::scroll', {
      data: {
        target,
        offset: 170,
      },
    })
  }

  /**
   * Checks if input type="number" fields contain text
   */
  numberFieldContainsText(input: InputType) {
    if (input.getAttribute('type') === 'number') {
      return !/^\d+$/.test(input.value)
    }
    return false
  }

  /**
   * Process API error
   */
  apiErrorHandler(error: any) {
    if (!error) return

    const errorMessage =
      error.error && typeof error.error !== 'string'
        ? Object.values(error.error)[0]
        : error.error || error.errorMessage || error.message || error.responseJSON

    if (!errorMessage) return
  }

  submitFormError(error: AxiosError<any>) {
    Events.$trigger('loader::hide')

    if (error.response?.data) {
      this.apiErrorHandler(error.response?.data)
    } else {
      throw new Error(`JAVASCRIPT ERROR: ${error}`)
    }

    if (typeof this.afterSubmitFormError === 'function') {
      this.afterSubmitFormError(error)
    }
  }

  /**
   * Submit form success handler
   */
  submitFormSuccess(data: Record<string, any>) {
    // If redirect url > redirect to page
    const redirectUrl = data.redirectUrl ?? data.data?.redirectUrl

    if (redirectUrl) {
      window.location.href = redirectUrl
      return
    }

    // Handle server field errors from body payload
    if (data.errors) {
      this.validateFormError(data.errors)
    }
    // If extended form provides afterSubmitFormSuccess or afterSubmitFormError function, this will be called
    if (typeof this.afterSubmitFormSuccess === 'function') {
      this.afterSubmitFormSuccess(data)
    }
  }

  getFormMethod() {
    const method = this.form.getAttribute('method')

    return (
      method && SUPPORTED_METHODS.includes(method.toLowerCase() as SupportedMethod)
        ? method.toLowerCase()
        : DEFAULT_METHOD
    ) as SupportedMethod
  }

  submitForm<T>(data: T) {
    API[this.method]<T>(this.action, data)
      .then(response => {
        this.submitFormSuccess(response.data)
      })
      .catch(error => {
        this.submitFormError(error)
      })
  }

  /**
   * Validate field success handler
   */
  validateFieldSuccess(input: InputType) {
    this.showInputSuccess(input)
  }

  /**
   * Validate field error handler
   */
  validateFieldError(input: InputType, message: ValidationRule['message']) {
    this.showInputError(input, message)
  }

  getErrorContainer(input: InputType) {
    const formItem = input.closest(FORM_ITEM_CLASS)
    return formItem?.querySelector(FORM_ITEM_ERROR_MESSAGE_HOOK)
  }

  /**
   * Show the error state and message
   */
  showErrorMessage(formItem: HTMLElement, input: InputType, message: ValidationRule['message']) {
    const formItemErrorContainer = this.getErrorContainer(input)
    input.setAttribute(FORM_ITEM_ERROR_ARIA, 'true')

    if (!formItemErrorContainer) return
    input.setAttribute(FORM_ITEM_DESCRIBE_ARIA, formItemErrorContainer.id)

    formItem.classList.add(FORM_ITEM_ERROR_CLASS)
    formItemErrorContainer.textContent = message
    formItemErrorContainer.classList.remove(HIDDEN_CLASS)
  }

  /**
   * Set validation error state on an input
   */
  showInputError(input: InputType, message: ValidationRule['message']) {
    const formItem = input.closest<HTMLElement>(FORM_ITEM_CLASS)
    if (!formItem) return
    this.resetFormItem(formItem, input)
    this.showErrorMessage(formItem, input, message)
  }

  /**
   * Set validation success state on an input
   */
  showInputSuccess(input: InputType) {
    const formItem = input.closest<HTMLElement>(FORM_ITEM_CLASS)
    if (!formItem) return

    this.resetFormItem(formItem, input)
    if (input.hasAttribute('required')) {
      formItem.classList.add(FORM_ITEM_SUCCESS_CLASS)
    }
  }

  /**
   * Remove error classes and any old error message
   */
  resetFormItem(formItem: HTMLElement, input: InputType) {
    const formItemErrorContainer = this.getErrorContainer(input)

    formItem.classList.remove(FORM_ITEM_ERROR_CLASS)
    formItem.classList.remove(FORM_ITEM_SUCCESS_CLASS)
    input.removeAttribute(FORM_ITEM_ERROR_ARIA)
    input.removeAttribute(FORM_ITEM_DESCRIBE_ARIA)

    if (formItemErrorContainer) {
      formItemErrorContainer.textContent = ''
      formItemErrorContainer.classList.add(HIDDEN_CLASS)
    }
  }

  /**
   * Validate field - do field validation
   */
  static validateField(input: InputType) {
    if (input.hasAttribute('data-ignored')) return

    const validations = input.getAttribute('data-validate')
    if (!validations) return

    return validations
      .split(',')
      .map(type => type.trim() as keyof typeof rules)
      .filter(type => !rules[type].method(input))
      .map(type => {
        const errorType = type === "email" ? 'pattern' : type
        const message = input.getAttribute(`data-${errorType}-error`)
        return message || rules[type].message
      })
      .join(', ')
  }

  /**
   * Handle onchange of an input
   */
  handleInputChange(input: InputType) {
    if (this.numberFieldContainsText(input)) input.value = ''

    const message = Form.validateField(input)

    if (message) return this.validateFieldError(input, message)
    return this.validateFieldSuccess(input)
  }

  getFormEntries() {
    return serializeObject(this.form)
  }

  getFormData() {
    return new FormData(this.form)
  }
  /**
   * Form validation success handler
   */
  validateFormSuccess() {
    if (this.showLoader) {
      Events.$trigger('loader::show', {
        data: {
          targetElement: this.form,
        },
      })
    }

    if (this.async) {
      return this.method === 'post'
        ? this.submitForm<any>(this.getFormData())
        : this.submitForm<any>(this.getFormEntries())
    } else {
      if (typeof this.beforeSubmitFormDefaultAction === 'function') {
        this.beforeSubmitFormDefaultAction(this.getFormData())
      }

      this.form.submit()
    }
  }

  /**
   * Show the input errors of the entire form per field and scroll to first invalid field
   *
   * @param {Object} messages | error messages returned from validation
   */
  showInputErrors(messages: FormErrorMessages) {
    let scrolledToError = false

    this.inputs.map(input => {
      if (!scrolledToError && messages[input.name]) {
        this.scrollToItem(input)
        scrolledToError = true
      }
      if (messages[input.name]) {
        return this.showInputError(input, messages[input.name])
      }
    })
  }

  /**
   * Form validation error handler
   */
  validateFormError(messages: FormErrorMessages) {
    this.showInputErrors(messages)
  }

  getErrorMessages() {
    return this.inputs.reduce(
      (acc, input) => {
        const message = Form.validateField(input)
        if (message) acc[input.name] = message
        return acc
      },
      {} as Record<string, string>,
    )
  }

  validateForm() {
    const messages = this.getErrorMessages()
    const isValid = !Object.keys(messages).length
    if (isValid) {
      return this.validateFormSuccess()
    }
    return this.validateFormError(messages)
  }

  /**
   * Submit the shipping form
   */
  handleFormSubmit(event: Event) {
    event.preventDefault()
    this.validateForm()
  }

  bindSubmitEvents() {
    // KEEP: For debug of form: submits the form without validation, to trigger backend errors right away.
    // this.form.addEventListener('submit', event => {
    //   event.preventDefault(event);
    //   this.submitForm();
    // });
    this.form.addEventListener('submit', event => this.handleFormSubmit(event))
  }

  /**
   * Bind change events of input elements
   */
  bindChangeEvents() {
    this.inputs.forEach(input =>
      this.events.forEach(event =>
        input.addEventListener(event, () => this.handleInputChange(input), false),
      ),
    )
  }

  rebindChangeEvents() {
    this.inputs.forEach(input =>
      this.events.forEach(event =>
        input.removeEventListener(event, () => this.handleInputChange(input), false),
      ),
    )

    this.bindChangeEvents()
  }

  resetForm() {
    this.inputs.forEach((input) => {
      const formItem = input.closest<HTMLElement>(FORM_ITEM_CLASS)
      if (!formItem) return
      this.resetFormItem(formItem, input)

      if (input.getAttribute('type') === 'checkbox' || input.getAttribute('type') === 'radio') {
        // @ts-ignore
        input.checked = false
      } else {
        input.value = ''
      }
    })
  }
}

export default Form
