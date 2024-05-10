import { Selector } from 'testcafe';

// Login test for staging environment
fixture('Login Test').page('http://74.234.8.67:5001/#/auth/login');

test('Valid Login', async t => {
    await t
        // Arrange (enter valid credentials)
        .typeText('#ion-input-0', 'user@example.com')
        .typeText('#ion-input-1', '12345678')
        
        // Act (click the SIGN IN button and let it load)
        .click('#btn-submit')
        .wait(1000)
        
        // Assert (check that app-home module is visible - only accessible once authenticated)
        .expect(Selector('body > app-root > ion-content > ion-router-outlet > app-home').filterVisible().exists).ok();
});

test('Invalid Login', async t => {
    await t
        // Arrange (enter invalid credentials)
        .typeText('#ion-input-0', 'user@example.com')
        .typeText('#ion-input-1', 'wrongPassword')

        // Act (click the SIGN IN button)
        .click('#btn-submit')
        .wait(1000)

        // Assert (check that the Sign in header is still visible, i.e. not left the login page)
        .expect(Selector('#register-card > ion-card-header > h1').filterVisible().exists).ok();
});