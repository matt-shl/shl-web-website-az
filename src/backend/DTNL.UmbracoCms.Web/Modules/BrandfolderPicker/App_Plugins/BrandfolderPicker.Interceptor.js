/**
 * MediaPickerBrandfolder Interceptor
 *
 * Intercepts the resolveFileFromEntity method to handle the Brandfolder Images.
 */
(function () {

  'use strict';

  angular.module("umbraco").run(function (mediaHelper, $log) {
    mediaHelper.resolveFileFromEntity = function (mediaEntity, thumbnail) {
      if (mediaEntity.metaData.ContentTypeAlias === "brandfolderImage" || mediaEntity.metaData.ContentTypeAlias === "brandfolderFile") {
        if (thumbnail) {
          return mediaEntity.metaData.MediaPath;
        }

        return mediaEntity.metaData.MediaPath.split("?")[0];
      }

      // default Umbraco implementation for resolveFileFromEntity
      var mediaPath = Utilities.isObject(mediaEntity.metaData) ? mediaEntity.metaData.MediaPath : null;
      return mediaPath ? thumbnail ? mediaHelper.detectIfImageByExtension(mediaPath) ? mediaHelper.getThumbnailFromPath(mediaPath) : "svg" === mediaHelper.getFileExtension(mediaPath) ? mediaHelper.getThumbnailFromPath(mediaPath) : null : mediaPath : ($log.warn("Cannot resolve the file url from the mediaEntity, it does not contain the required metaData"),
       null)
    };

  });

})();
