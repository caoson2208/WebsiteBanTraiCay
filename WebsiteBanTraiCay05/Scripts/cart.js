$(document).ready(function () {
    ShowCount();
    $('body').on('click', '#btnAddToCart', function (even) {
        even.preventDefault();
        var id = $(this).data('id');
        var quantity = 1;
        var quantityValue = $('#quantityValue').text();
        if (quantityValue !== '') {
            quantity = parseInt(quantityValue);
        }
        $.ajax({
            url: '/cart/addtocart',
            type: 'POST',
            data: { id: id, quantity: quantity },
            success: function (res) {
                if (res.Success) {
                    $('#checkoutItems').html(res.Count);
                    Swal.fire({
                        position: "center",
                        icon: "success",
                        title: "Thêm vào giỏ thành công!",
                        showConfirmButton: false,
                        timer: 1000
                    })
                }
            }
        });
    });

    $('body').on('click', '#btnUpdate', function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        var quantity = $('#Quantity_' + id).val();
        Update(id, quantity);
    });

    $('body').on('click', '#btnDelete', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        Swal.fire({
            title: "Bạn có muốn xóa sản phẩm này khỏi giỏ hàng?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonColor: "#3085d6",
            cancelButtonColor: "#d33",
            confirmButtonText: "OK"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: '/cart/delete',
                    type: 'POST',
                    data: { id: id },
                    success: function (res) {
                        if (res.Success) {
                            $('#checkoutItems').html(res.Count);
                            $('#trow_' + id).remove();
                            Swal.fire({
                                position: "center",
                                icon: "success",
                                title: "Xoá thành công!",
                                showConfirmButton: false,
                                timer: 1000,
                            }).then(() => {
                                setTimeout(() => {
                                    location.reload(true);
                                }, 100);
                            });
                        }
                    }
                });
            }
        });
    });

});

function ShowCount() {
    $.ajax({
        url: '/cart/ShowCount',
        type: 'GET',
        success: function (res) {
            $('#checkoutItems').html(res.Count);
        }
    });
}

function Update(id, quantity) {
    $.ajax({
        url: '/cart/Update',
        type: 'POST',
        data: { id: id, quantity: quantity },
        success: function (res) {
            if (res.Success) {
                Swal.fire({
                    position: "center",
                    icon: "success",
                    title: "Sửa thành công!",
                    showConfirmButton: false,
                    timer: 1000
                }).then(() => {
                    setTimeout(() => {
                        location.reload(true);
                    }, 100);
                });
            }
        }
    });
}

function LoadCart() {
    $.ajax({
        url: '/cart/Partial_CartItem',
        type: 'GET',
        success: function (res) {
            $('#loadData').html(res);
        }
    });
}

