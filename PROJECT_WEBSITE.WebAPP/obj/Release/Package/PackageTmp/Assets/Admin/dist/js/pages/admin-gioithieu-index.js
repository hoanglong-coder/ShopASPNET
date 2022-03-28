
//Thông báo
var Toast = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000
});

//Main
var NewsController = {
    init: function () {

       
        NewsController.loadGioiThieu();

        NewsController.Luu();
    },
    loadGioiThieu: function () {
        CKEDITOR.replace('NoiDung', {
            customConfig: '/Assets/Admin/plugins/ckeditor/config.js'
        });

        $.ajax({
            type: "get",
            url: '/admin/gioithieu/LoadGioiThieu',
            dataType: 'json',
            success: function (data) {
                if (data.data != null || data.data != '') {
                    CKEDITOR.instances['NoiDung'].setData(data.data);
                }
            }
        });

    },
    Luu: function () {
        $("#Luu").off('click').on('click', function () {

            var txt = CKEDITOR.instances['NoiDung'].getData();

            $.ajax({
                type: "post",
                url: '/admin/gioithieu/EditGioiThieu',
                data: {
                    noidung:txt
                },
                dataType: 'json',
                success: function (data) {
                    if (data.code == 200) {
                        if (data.data == true) {
                            Toast.fire({
                                icon: 'success',
                                title: 'Thành công',
                            })
                        } else {
                            Toast.fire({
                                icon: 'error',
                                title: 'Thất bại',
                            })
                        }
                    }
                }
            });

        })

    }
}
NewsController.init();