    document.addEventListener("DOMContentLoaded", function () {
        const starRatings = document.querySelectorAll(".star-rating");

        starRatings.forEach(starRating => {
            const stars = starRating.querySelectorAll(".star");
    const hiddenInput = document.getElementById("ReviewRate-" + starRating.dataset.reviewId);

            stars.forEach(star => {
        star.addEventListener("click", function () {
            const value = this.dataset.value;

            // Set the value in the hidden input
            hiddenInput.value = value;

            // Highlight the selected stars
            stars.forEach(s => s.classList.remove("selected"));
            for (let i = 0; i < value; i++) {
                stars[i].classList.add("selected");
            }
        });

    // Optional: Hover effect for previewing the selection
    star.addEventListener("mouseover", function () {
                    const value = this.dataset.value;

                    stars.forEach(s => s.classList.remove("selected"));
    for (let i = 0; i < value; i++) {
        stars[i].classList.add("selected");
                    }
                });

    starRating.addEventListener("mouseleave", function () {
                    // Restore selection on mouse leave
                    const currentValue = hiddenInput.value;
                    stars.forEach(s => s.classList.remove("selected"));
    for (let i = 0; i < currentValue; i++) {
        stars[i].classList.add("selected");
                    }
                });
            });
        });
    });
