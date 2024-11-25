export const jsonp = (url: string, callback: (arg0: any) => void) => {
  const callbackName = 'jsonp_callback_' + Math.round(100000 * Math.random());
  // @ts-ignore
  window[callbackName] = (data) => {
    // @ts-ignore
    delete window[callbackName];
    document.body.removeChild(script);
    callback(data);
  };

  const script = document.createElement('script');
  script.src = url + (url.indexOf('?') >= 0 ? '&' : '?') + 'callback=' + callbackName;
  script.type = 'text/javascript';
  document.body.appendChild(script);
}
