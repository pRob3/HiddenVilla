redirectToCheckout = function (sessionId) {
    var stripe = Stripe('pk_test_51IQ46iCc3UUbcV0JRbjDeQeckIojs0T1Z49Awu9qi76BXl4l9XpLXJ3yaD5WVsFay6kJWcp2e8iq1kkYN20c8LnN00IzdOKrCO');
    stripe.redirectToCheckout({
        sessionId: sessionId
    });
}