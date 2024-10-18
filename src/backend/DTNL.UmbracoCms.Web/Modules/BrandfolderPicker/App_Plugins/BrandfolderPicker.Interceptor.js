/**
 * MediaPickerBrandfolder Interceptor
 *
 * Intercepts the resolveFileFromEntity method to handle the Brandfolder Images.
 */
(function () {

  'use strict';

  angular.module("umbraco").run(function (mediaHelper, $http, $rootScope, $timeout) {

    var defaultResolveFileFromEntity = mediaHelper.resolveFileFromEntity

    mediaHelper.resolveFileFromEntity = function (mediaEntity, isThumbnail) {

      if (mediaEntity.metaData.ContentTypeAlias === "brandfolderImage") {
        if (isThumbnail) {
          return mediaEntity.metaData.MediaPath;
        }

        return mediaEntity.metaData.MediaPath.split("?")[0];
      }

      return defaultResolveFileFromEntity(mediaEntity, isThumbnail);
    };

  });

})();
