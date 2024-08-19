import Events from '@utilities/events'

const JS_HOOK_ODOMETER_DIGIT_CONTAINER = '[js-hook-odometer-digit-container]'
const JS_ATTR_ODOMETER_DIGIT = 'data-number'

class Odometer {
  element: HTMLElement
  digitContainers: HTMLElement[] | null
  digitsElements: HTMLElement[] | null
  elementId: string

  constructor(element: HTMLElement) {
    this.element = element
    this.elementId = this.element.id
    this.digitContainers = [
      ...this.element.querySelectorAll<HTMLElement>(JS_HOOK_ODOMETER_DIGIT_CONTAINER),
    ]
    this.digitsElements = [
      ...this.element.querySelectorAll<HTMLElement>(`[${JS_ATTR_ODOMETER_DIGIT}]`),
    ]
    this.#bindEvents()
  }

  #bindEvents() {
    Events.$on(`odometer[${this.elementId}]::setNumber`, (_, data: { number: number }) => {
      this.#setNumber(data.number)
    })
  }

  #setNumber(number: number) {
    const numberArray = number.toString().split('')
    this.digitsElements?.forEach((el, index) => {
      const num = numberArray[index] || '0'
      ;(el as HTMLElement).setAttribute(JS_ATTR_ODOMETER_DIGIT, num)
    })
  }
}

export default Odometer
