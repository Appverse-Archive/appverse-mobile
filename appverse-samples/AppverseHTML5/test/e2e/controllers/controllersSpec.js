//
// test/e2e/controllers/controllersSpec.js
//
describe("E2E: Testing Controllers", function () {

    beforeEach(function () {
        browser().navigateTo('/');
    });

    it('should have a working home page controller', function () {
        browser().navigateTo('#/home');
        expect(browser().location().path()).toBe("/home");
        expect(element('[ui-view]').html()).toContain('ng-controller="homeController"');
    });

});
