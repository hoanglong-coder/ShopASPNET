var payment = {
    init: function () {
        payment.createTinh();
        payment.Addpayment();
        payment.regEvents();
        
    },
    regEvents: function () {


    }, createTinh: function () {
        var data = $.getJSON("/Assets/Client/data/dvhcvn.json", function () {
        }).done(function (data) {
            var $select = $('#tinh');
            $.each(data.data, function (key, val) {
                $('<option>').val(val.level1_id).text(val.name).appendTo($select);
                $('#tinh').val("79");
                payment.createQuan($('#tinh').val(), data.data);
                payment.createXa($('#tinh').val(), $('#quan').val(), data.data);
            });
            $('#tinh').on('change', function () {
                payment.createQuan($(this).val(), data.data);
                payment.createXa($('#tinh').val(), $('#quan').val(), data.data);
            });
            $('#quan').on('change', function () {
                payment.createXa($('#tinh').val(), $(this).val(), data.data);

            });
        });
    }, createQuan: function (idtinh, data) {
        var $select2 = $("#quan");
        $select2.find('option').remove();
        $.each(data, function (key, val) {
            if (val.level1_id == idtinh) {
                $.each(val.level2s, function (key, val) {
                    $('<option>').val(val.level2_id).text(val.name).appendTo($select2);
                });
            }

        });
    }, createXa: function (idtinh, idquan, data) {
        var $select3 = $("#xa");
        $select3.find('option').remove();
        $.each(data, function (key, val) {
            if (val.level1_id == idtinh) {
                $.each(val.level2s, function (key, val) {
                    if (val.level2_id == idquan) {
                        $.each(val.level3s, function (key, val) {
                            $('<option>').val(val.level3_id).text(val.name).appendTo($select3);
                        });
                    }
                });
            }
        });
    }, Addpayment: function () {
        $('#btnSuccess').off('click').on('click', function () {
            var ordert = new Object();
            ordert.CreatedDate = Date.now();
            ordert.ShipName = $('#txtName').val();
            ordert.ShipMobile = $('#txtNumber').val();
            ordert.ShipEmail = $("#txtEmailShip").val();
            ordert.ShipAddress = $("#txtAddress").val() + "+" + $("#xa :selected").text() + "+" + $("#quan :selected").text() + "+" + $("#tinh :selected").text();
            ordert.Discription = $('#txtdescription').val();
            ordert.PayStatus = ($("input[name=Payment]:checked").val() == 0) ? false : true;
            ordert.Status = -1;

            var check = $("input:radio[name ='Payment']:checked").val();
            if (check == 1) {
                if (ordert != null) {
                    $.ajax({
                        url: '/Cart/Paymentonline',
                        type: 'post',
                        data: ordert,
                        dataType: 'json',
                        success: function (data) {
                            if (data.code == 200) {
                                window.location.href = data.PayUrl;
                            }
                        }
                    });
                }
            } else {
                if (ordert != null) {
                    $.ajax({
                        url: '/Cart/PaymentCOD',
                        type: 'post',
                        data: ordert,
                        dataType: 'json',
                        success: function (data) {
                            if (data.status) {
                                window.location.href = "/Cart/FinalOrderCOD";
                            } else {

                            }
                          
                        }
                    });
                }
            }       
        });
    }

}
payment.init();