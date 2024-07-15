import { isValidEmail, isValidFile, isValidIBAN, isValidZipcode } from './validation'

/**
 * A set of generic form validation rules which you can use on a form input as:
 * data-validate="required,email" or data-validate="iban"
 * Customize the way you want with your own validation rules
 *
 * How To Do Translations?
 * There are many ways to do this. For example:
 *
 * 1. add a json with messages in a script tag somewhere in the page:
 * window.validationMessages = { required: 'Field is required', email: 'Email is not valid', zipcode: 'Zipcode is not valid' }
 *
 * 2. use the validationMessages object in here to render out the translated messages:
 * const rules = { required: { message: window.validationMessages.required, ..etc } }
 */
const rules = {
  required: {
    message: 'Veld is verplicht',
    method: (el: HTMLInputElement) => {
      if (el.type === 'checkbox') {
        return el.checked
      } else if (el.type === 'radio') {
        const name = el.name
        return el.parentNode!.querySelectorAll(`input[name=${name}]:checked`).length > 0
      }
      return el.value !== ''
    },
  },
  email: {
    message: 'Geen geldig e-mailadres',
    method: (el: HTMLInputElement) => el.value === '' || isValidEmail(el.value),
  },
  iban: {
    message: 'Geen geldig IBAN nummer',
    method: (el: HTMLInputElement) => el.value === '' || isValidIBAN(el.value),
  },
  zipcode: {
    message: 'Geen geldige postcode',
    method: (el: HTMLInputElement) => el.value === '' || isValidZipcode(el.value),
  },
  file: {
    message: 'Geen geldig bestand',
    method: (el: HTMLInputElement) => el.value === '' || isValidFile(el),
  },
}

export { rules }
