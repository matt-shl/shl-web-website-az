import Form from "@components/form";

class PardotForm extends Form {
  element: HTMLFormElement
  hiddenInputs: HTMLInputElement[]

  constructor(element: HTMLFormElement) {
    super(element)

    this.hiddenInputs = [...element.querySelectorAll<HTMLInputElement>('input[type="hidden"]')]
  }

  submitForm = (_data: any) => {
    const inputsMapped = [...this.inputs, ...this.hiddenInputs].map(fe => ({ key: fe.name, value: fe.value }));
    const queryParams = inputsMapped.map(({ key, value }) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
      .join('&');

    window.location.href = `${this.action}?${queryParams}`
  }
}

export default PardotForm
