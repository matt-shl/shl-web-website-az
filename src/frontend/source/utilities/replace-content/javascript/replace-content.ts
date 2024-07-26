import environment from '@utilities/environment'
import Events from '@utilities/events'

const JS_ATTRIBUTE_REPLACE_CONTENT = 'js-hook-replace-content'
const JS_HOOK_REPLACE_CONTENT = `[${JS_ATTRIBUTE_REPLACE_CONTENT}]`

class ReplaceContent {
  constructor() {
    console.log('ReplaceContent', [...document.querySelectorAll(JS_HOOK_REPLACE_CONTENT)])
  }

  #getContentIds(newContent: Element) {
    return [...newContent.querySelectorAll(JS_HOOK_REPLACE_CONTENT)].map(element =>
      element.getAttribute(JS_ATTRIBUTE_REPLACE_CONTENT),
    )
  }

  replaceAllContent(newContent: Element) {
    const newContentIds = this.#getContentIds(newContent)

    const replaceContentElements = [...document.querySelectorAll(JS_HOOK_REPLACE_CONTENT)].filter(
      element =>
        newContentIds.includes(element.getAttribute(JS_ATTRIBUTE_REPLACE_CONTENT)) && element,
    )

    for (const element of replaceContentElements) {
      const replacementElement = newContent.querySelector(
        `[${JS_ATTRIBUTE_REPLACE_CONTENT}="${element.getAttribute(JS_ATTRIBUTE_REPLACE_CONTENT)}"]`,
      )
      if (replacementElement) {
        element.replaceWith(replacementElement)
      }
    }

    Events.$trigger('loader::hide')
    Events.$trigger('replaceContent::replacedContentComplete')
  }
}

export default new ReplaceContent()
