import Form from "@components/form";
import Events from "@utilities/events";

class PardotForm extends Form {
  element: HTMLFormElement
  hiddenInputs: HTMLInputElement[]
  private gtmData: string | undefined;

  constructor(element: HTMLFormElement) {
    super(element)

    this.initGtmData()

    this.hiddenInputs = [...element.querySelectorAll<HTMLInputElement>('input[type="hidden"]')]
  }

  initGtmData() {
    const gtmData = this.form.dataset.gtm;
    this.gtmData = (gtmData !== "undefined" && gtmData !== "" && gtmData) ? JSON.parse(gtmData.replace(/'/g, '"')) : undefined
  }

  submitForm = (_data: any) => {
    const inputsMapped = [...this.inputs, ...this.hiddenInputs].map(fe => ({ key: fe.name, value: fe.value }));
    const queryParams = inputsMapped.map(({ key, value }) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
      .join('&');

    if(this.gtmData) {
      Events.$trigger('gtm::push', {
        data: this.gtmData
      })
    }

    window.location.href = `${this.action}?${queryParams}`
  }
}

export default PardotForm
