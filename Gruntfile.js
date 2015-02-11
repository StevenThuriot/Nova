/*
 * grunt-cleanDotNet
 * https://github.com/StevenThuriot/grunt-cleanDotNet
 *
 * Copyright (c) 2014 Steven Thuriot
 * Licensed under the MIT license.
 */

'use strict';

module.exports = function(grunt) {

  grunt.loadNpmTasks('grunt-cleanDotNet');
  grunt.registerTask('default', ['cleanDotNet']);
};
