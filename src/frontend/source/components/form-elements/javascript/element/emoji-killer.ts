const EMOJI_REGEX = /\p{Extended_Pictographic}/gu

/**
 * Kill pesky emoji's in your input fields. Pass a single element, array of elements or NodeList.
 * Binds events that will strip your input of Emoji's.
 * Instantiate by calling EmojiKiller() and pass a single element, an array of elements or NodeList to it.
 */

const emojiKiller = (element: HTMLInputElement | HTMLInputElement[]) => {
  const elementArray = Array.isArray(element) ? element : [element]

  elementArray.forEach(input => {
    input.addEventListener('change', () => preventEmoji(input))
    input.addEventListener('input', () => preventEmoji(input))
  })
}

const preventEmoji = (domElement: HTMLInputElement) => {
  domElement.value = domElement.value.replace(EMOJI_REGEX, '')
}

export default emojiKiller
