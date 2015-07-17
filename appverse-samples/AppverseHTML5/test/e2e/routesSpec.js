/*jshint node:true */
'use strict';

describe("E2E: Testing Routes", function () {

    beforeEach(function () {
        browser().navigateTo('/');
    });

    it('should jump to the /home path when / is accessed', function () {
        browser().navigateTo('#/');
        expect(browser().location().path()).toBe("/home");
    });

    it('should have a working /home route', function () {
        browser().navigateTo('#/home');
        expect(browser().location().path()).toBe("/home");
    });

});
