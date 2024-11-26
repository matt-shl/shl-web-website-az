import Form from "@components/form";
import Events from "@utilities/events";

class PardotForm extends Form {
  element: HTMLFormElement
  private successText: string;
  private errorText: string;
  private iframe: HTMLIFrameElement;

  constructor(element: HTMLFormElement) {
    super(element)

    this.iframe = this.form.querySelector("iframe[name='responseFrame']") as HTMLIFrameElement;
    this.successText = this.form.dataset.successText || 'Success'
    this.errorText = this.form.dataset.errorText || 'Sorry, something went wrong.'

    this.bindEvents()
  }

  bindEvents() {
    this.iframe.addEventListener("load",  () => this.loadIframe());
  }

  loadIframe() {
    if(!this.iframe.src) return
    //@ts-ignore
    const iframeDoc = this.iframe.contentDocument || this.iframe.contentWindow.document;
    const responseText = iframeDoc.body.textContent;

    if(!responseText) return

    Events.$trigger('loader::hide')
    // this.iframe.src = '' // Temp turn off to keep response visible for testing

    if (responseText.includes('submitForm=true')) {
      this.afterSubmitFormSuccess(responseText)
    } else {
      this.afterSubmitFormError(responseText)
    }
  }

  submitForm = (_data: any) => {
    const inputsMapped = this.inputs.map(fe => ({ key: fe.name, value: fe.value }));
    const queryParams = inputsMapped.map(({ key, value }) => `${encodeURIComponent(key)}=${encodeURIComponent(value)}`)
      .join('&');
    this.iframe.src = `${this.action}?${queryParams}`
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
