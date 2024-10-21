/**
 * MediaPickerBrandfolder Interceptor
 *
 * Intercepts the resolveFileFromEntity method to handle the Brandfolder Images.
 */
(function () {

  'use strict';

  angular.module("umbraco").run(function (mediaHelper, $log) {
    mediaHelper.resolveFileFromEntity = function (mediaEntity, thumbnail) {
      if (mediaEntity.metaData.ContentTypeAlias === "brandfolderImage") {
        if (thumbnail) {
          return mediaEntity.metaData.MediaPath;
        }

        return mediaEntity.metaData.MediaPath.split("?")[0];
      }

      var mediaPath = Utilities.isObject(mediaEntity.metaData) ? mediaEntity.metaData.MediaPath : null;
      return mediaPath ? thumbnail ? mediaHelper.detectIfImageByExtension(mediaPath) ? mediaHelper.getThumbnailFromPath(mediaPath) : "svg" === mediaHelper.getFileExtension(mediaPath) ? mediaHelper.getThumbnailFromPath(mediaPath) : null : mediaPath : ($log.warn("Cannot resolve the file url from the mediaEntity, it does not contain the required metaData"),
       null)
    };

  });

})();
