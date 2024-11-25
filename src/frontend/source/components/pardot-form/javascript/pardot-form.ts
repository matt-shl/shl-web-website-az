import Form from "@components/form";
import Events from "@utilities/events";
import { jsonp } from "@utilities/jsonp";

class PardotForm extends Form {
  element: HTMLFormElement
  private successText: string;
  private errorText: string;

  constructor(element: HTMLFormElement) {
    super(element)

    this.successText = this.form.dataset.successText || 'Success'
    this.errorText = this.form.dataset.errorText || 'Sorry, something went wrong.'
  }

  submitForm = (_data: any) => {
    const inputsMapped = this.inputs.map(fe => ({ key: fe.name, value: fe.value }));
    const queryParams = inputsMapped.map(({ key, value }) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
      .join('&');
    this.action = `${this.action}?${queryParams}`;

    // let jsonp submit the form
    jsonp(this.action, (data) => {
      Events.$trigger('loader::hide')

      console.log(data); // Temp, keep in here to be able to test on test env

      // see if 'submitform=true' is in the returned data (URL)
      if (data.indexOf('submitform=true') > -1) {
        this.afterSubmitFormSuccess(data)
      } else {
        this.afterSubmitFormError(data)
      }

    })
  }

  afterSubmitFormError = (_data: any) => {
    Events.$trigger('toastManager::add', {
      data: { title: this.errorText, status: 'error' },
    })
  }

  afterSubmitFormSuccess = (_data: any) => {
    Events.$trigger('toastManager::add', {
      data: { title: this.successText, status: 'success' },
    })

    this.resetForm()
  }
}

export default PardotForm
