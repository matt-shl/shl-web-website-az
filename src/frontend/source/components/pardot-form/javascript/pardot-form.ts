import Form from "@components/form";

class PardotForm extends Form {
  element: HTMLFormElement

  constructor(element: HTMLFormElement) {
    super(element)
  }

  submitForm = (_data: any) => {
    const inputsMapped = this.inputs.map(fe => ({ key: fe.name, value: fe.value }));
    const queryParams = inputsMapped.map(({ key, value }) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
      .join('&');

    window.location.href = `${this.action}?${queryParams}`
  }
}

export default PardotForm
