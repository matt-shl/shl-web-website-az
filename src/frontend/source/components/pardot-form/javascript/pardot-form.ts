import Form from "@components/form";
import Events from "@utilities/events";

class PardotForm extends Form {
  element: HTMLFormElement
  private successText: string;
  private errorText: string;

  constructor(element: HTMLFormElement) {
    super(element)

    this.successText = this.form.dataset.successText || 'Success'
    this.errorText = this.form.dataset.errorText || 'Sorry, something went wrong.'
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

  submitForm = (_data: any) => {
    const inputsMapped = this.inputs.map(fe => ({ key: fe.name, value: fe.value }));
    const queryParams = inputsMapped.map(({ key, value }) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
      .join('&');
    this.action = `${this.action}&${queryParams}`;
    super.submitForm(_data)
  }
}

export default PardotForm
