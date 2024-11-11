import {html} from "@utilities/dom-elements";
import Environment from "@utilities/environment";
import { LocalStorage, SessionStorage } from '@utilities/storage'

const CLASS_HAS_LOAD_ANIMATION = "has--page-load-animation"
const STORAGE_KEY_FIRST_TIME_VISITING = 'firstTimeVisiting'
const IS_REDUCED_MOTION_CLASS = 'is--reduced-motion'

const STORAGE_KEY_IGNORE_FIRST_TIME_VISITING = 'ignore'
const STORAGE_VALUE_IGNORE_FIRST_TIME_VISITING = 'please'

class PageLoadAnimation {
  private isFirstTimeVisiting: boolean;
  private isIgnoreFirstTimeVisiting: boolean;
  private lastAnimationItem: Element | null;
  private isDebugging: boolean;

  constructor() {
    if (html.classList.contains(IS_REDUCED_MOTION_CLASS)) return;

    this.isFirstTimeVisiting = SessionStorage.get(STORAGE_KEY_FIRST_TIME_VISITING) === null
    this.isIgnoreFirstTimeVisiting = LocalStorage.get(STORAGE_KEY_IGNORE_FIRST_TIME_VISITING) === STORAGE_VALUE_IGNORE_FIRST_TIME_VISITING
    this.lastAnimationItem = PageLoadAnimation.getLastAnimationItem()

    const params = new URLSearchParams(window.location.search)
    this.isDebugging = params.get('debug') !== null && (Environment.isLocal || Environment.isTest);

    this.bindEvents();
    this.init();
  }

  static getLastAnimationItem() {
    return document.querySelector('.hero-home__cta') || document.querySelector('.hero-home__title')
  }

  bindEvents() {
    this.lastAnimationItem?.addEventListener('animationend', () => {
      html.classList.remove(CLASS_HAS_LOAD_ANIMATION)
    });
  }

  init() {
    if (this.isFirstTimeVisiting || this.isDebugging) {
      if(this.isIgnoreFirstTimeVisiting) {
        console.warn(`Ignoring first time visit animation because of LocalStorage key '${STORAGE_KEY_IGNORE_FIRST_TIME_VISITING}' is set to '${STORAGE_VALUE_IGNORE_FIRST_TIME_VISITING}'`);
        return;
      }

      html.classList.add(CLASS_HAS_LOAD_ANIMATION)
      SessionStorage.set(STORAGE_KEY_FIRST_TIME_VISITING, "false")

      if (this.isDebugging) {
        setTimeout(() => {
          document.documentElement.scrollTop = 0;
        }, 100)
      }
    }
  }
}

export default new PageLoadAnimation()
