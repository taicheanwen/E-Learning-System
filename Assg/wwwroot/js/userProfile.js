// Photo preview
$('.upload input').on('change', e => {
    const f = e.target.files[0];
    const img = $(e.target).siblings('img')[0];

    img.dataset.src ??= img.src;

    if (f && f.type.startsWith('image/')) {
        img.onload = () => {
            URL.revokeObjectURL(img.src);
        };
        img.src = URL.createObjectURL(f);
    }
    else {
        img.src = img.dataset.src;
        e.target.value = '';
    }
    $(e.target).valid();
});

$(document).ready(function () {
    $("#profileForm").submit(function (e) {
        e.preventDefault();

        var formData = new FormData(this);

        $.ajax({
            url: '/Account/EditProfile',
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.success) {
                    $('#profileImage').attr('src', response.newProfileImageUrl + '?timestamp=' + new Date().getTime());
                    alert('Profile updated successfully!');
                } else {
                    alert(response.message || 'Error updating profile.');
                }
            },
            error: function () {
                alert('Error updating profile.');
            }
        });
    });
});

