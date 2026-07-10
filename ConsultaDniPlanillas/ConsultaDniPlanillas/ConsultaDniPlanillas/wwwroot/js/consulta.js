let boletaSeleccionadaGlobal = null;

const API_BASE = "http://192.168.4.91:203/api/ConsultaDni";

document.getElementById("searchForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const dni = document.getElementById("dni").value;
    const anio = document.getElementById("anio").value;
    const mesDesde = document.getElementById("mesDesde").value;
    const mesHasta = document.getElementById("mesHasta").value;

    let url = `${API_BASE}/historial/${dni}?`;

    if (anio) url += `anio=${anio}&`;
    if (mesDesde) url += `mesDesde=${mesDesde}&`;
    if (mesHasta) url += `mesHasta=${mesHasta}`;

    try {
        document.getElementById("loadingSpinner").classList.remove("d-none");

        const response = await fetch(url, {
            method: "GET",
            headers: { "Accept": "application/json" }
        });

        if (!response.ok) throw new Error("Error al consultar la API");

        const data = await response.json();
        cargarTabla(data);

    } catch (error) {
        console.error(error);
        alert(error.message);
    } finally {
        document.getElementById("loadingSpinner").classList.add("d-none");
    }
});

function cargarTabla(lista) {
    const tbody = document.querySelector("#boletasTable tbody");
    tbody.innerHTML = "";

    if (lista.length === 0) {
        document.getElementById("resultsCard").classList.add("d-none");
        return;
    }

    lista.forEach(item => {
        const pMes = item.mes.toString().padStart(2, '0');
        const periodo = `${pMes}/${item.anio}`;

        const regimenDesc = item.adminPens || item.tipoPension || item.regimenPensionary || "-";
        const codEmp = item.codEmpleado || item.idEmpleado;
        const situacion = item.situacion || "-";

        let badgeColor = "bg-secondary bg-opacity-10 text-secondary";

        if (situacion.toUpperCase() === "ACTIVO") {
            badgeColor = "bg-success bg-opacity-10 text-success";
        } else if (situacion.toUpperCase() === "INACTIVO") {
            badgeColor = "bg-danger bg-opacity-10 text-danger";
        }

        const itemString = JSON.stringify(item).replace(/'/g, "&@@apos;");

        tbody.innerHTML += `
                <tr>
                    <td class="ps-4 fw-semibold text-dark">${periodo}</td>
                    <td>${item.dni}</td>
                    <td><span class="badge bg-light text-secondary border px-2 py-1">${codEmp}</span></td>
                    <td class="fw-medium text-dark">${item.nombresCompletos}</td>
                    <td>${item.condicionLaboral}</td>
                    <td>${item.cargo}</td>
                    <td>${item.centroCosto}</td>
                    <td>${item.fechaIngreso}</td>
                    <td>${regimenDesc}</td>
                    <td class="text-center">
                        <span class="badge ${badgeColor} px-3 py-2 rounded-pill fw-semibold small">${situacion}</span>
                    </td>
                    <td class="text-center pe-4">
    <button class="btn btn-xs btn-light text-primary border border-primary-subtle px-2 py-1 rounded-2 shadow-sm d-inline-flex align-items-center justify-content-center transition-all button-hover-modern" 
            onclick='verBoleta(${itemString})' 
            title="Ver Boleta" 
            style="font-size: 0.7rem; gap: 4px; font-weight: 600; letter-spacing: 0.3px; height: 26px;">
        <i class="fas fa-eye" style="font-size: 0.75rem;"></i>
        <span>Ver</span>
    </button>
</td>
                </tr>`;
    });

    document.getElementById("resultsCard").classList.remove("d-none");
}

function verBoleta(item) {
    boletaSeleccionadaGlobal = item;
    const mesesNombres = ["ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"];
    const mesTexto = mesesNombres[parseInt(item.mes) - 1] || "MES";

    $("#lblPeriodoTitulo").text(`${mesTexto} - ${item.anio}`);
    $("#lblTituloPlanilla").text(item.nombrePlanilla ? item.nombrePlanilla.toUpperCase() : "PLANILLA CAS INDETERMINADOS");

    $("#lblEntidad").text(item.entidad || "Municipalidad Distrital de Ate");
    $("#lblEmpleador").text(item.empleador || "Municipalidad Distrital de Ate");
    $("#lblRuc").text(item.ruc || "20131378620");
    $("#lblRubro").text(item.rubro || "-");
    $("#lblMeta").text(item.meta || "-");
    $("#lblUnidadOrganica").text(item.centroCosto || "-");

    $("#lblDni").text(item.dni);
    $("#lblAirhsp").text(item.airhsp || "-");
    $("#lblNombre").text(item.nombresCompletos);
    $("#lblCargo").text(item.cargo);
    $("#lblFechaIngreso").text(item.fechaIngreso);
    $("#lblRegimen").text(item.tipoPension || "-");
    $("#lblAdminPens").text(item.adminPens || "-");
    $("#lblCuspp").text(item.cuspp || "-");

    $("#lblSede").text(item.sede || "PALACIO MUNICIPAL");
    $("#lblRegimenLaboral").text(item.condicionLaboral || "-");
    $("#lblCondicion").text(item.condicion || "-");
    $("#lblOcupacional").text(item.ocupacional || "-");
    $("#lblEstructural").text(item.estructural || "-");
    $("#lblCodEmpleado").text(item.codEmpleado || item.idEmpleado);
    $("#lblTipoComision").text(item.tipoComision || "-");

    $("#lblJornada").text(item.jornada || 0);
    $("#lblDiasLab").text(item.diasLaborados || 0);
    $("#lblDiasNoLab").text(item.diasNoLaborados || 0);
    $("#lblSubsidios").text(item.subsidios || 0);
    $("#lblVacaciones").text(item.vacaciones || 0);

    $("#lblTotalIngresos").text(item.totalIngresos.toFixed(2));
    $("#lblTotalEgresos").text(item.totalEgresos.toFixed(2));
    $("#lblNetoPagar").text(item.netoPagar.toFixed(2));

    const ingresosArr = typeof item.ingresos === 'string' ? JSON.parse(item.ingresos || "[]") : (item.ingresos || []);
    const egresosArr = typeof item.egresos === 'string' ? JSON.parse(item.egresos || "[]") : (item.egresos || []);
    const aportesArr = typeof item.aportes === 'string' ? JSON.parse(item.aportes || "[]") : (item.aportes || []);

    let htmlIngresos = "";
    ingresosArr.forEach(i => {
        htmlIngresos += `
            <tr>
                <td class="ps-3">${i.codigoInterno} - ${i.concepto}</td>
                <td class="text-end pe-3 fw-medium text-success">+ S/ ${i.monto.toFixed(2)}</td>
            </tr>`;
    });
    $("#tblModalIngresos tbody").html(htmlIngresos || "<tr><td colspan='2' class='text-muted text-center py-3'>Sin registros</td></tr>");

    let htmlEgresos = "";
    egresosArr.forEach(i => {
        htmlEgresos += `
            <tr>
                <td class="ps-3">${i.codigoInterno} - ${i.concepto}</td>
                <td class="text-end pe-3 fw-medium text-danger">- S/ ${i.monto.toFixed(2)}</td>
            </tr>`;
    });
    $("#tblModalEgresos tbody").html(htmlEgresos || "<tr><td colspan='2' class='text-muted text-center py-3'>Sin registros</td></tr>");

    let htmlAportes = "";
    let totalAportes = 0;
    aportesArr.forEach(i => {
        totalAportes += i.monto;
        htmlAportes += `
            <tr>
                <td class="ps-3">${i.codigoInterno} - ${i.concepto}</td>
                <td class="text-end pe-3 fw-medium text-primary">S/ ${i.monto.toFixed(2)}</td>
            </tr>`;
    });
    $("#tblModalAportes tbody").html(htmlAportes || "<tr><td colspan='2' class='text-muted text-center py-2'>Sin registros</td></tr>");
    $("#lblTotalAportes").text(totalAportes.toFixed(2));

    const modal = new bootstrap.Modal(document.getElementById("boletaModal"));
    modal.show();
}




function verBoletaPDFEnPestana(item) {
    if (!item) {
        alert("No hay datos válidos para generar la boleta.");
        return;
    }

    let jsPDFWindow = window.jspdf ? window.jspdf.jsPDF : window.jsPDF;

    if (!jsPDFWindow) {
        alert("La librería jsPDF no está cargada correctamente. Verifica tus scripts.");
        return;
    }


    const doc = new jsPDFWindow({ orientation: 'portrait', unit: 'mm', format: 'a5' });
    const pageWidth = doc.internal.pageSize.width;

    const usuarioActivo = typeof window.g_UsuarioLoginCodigo !== 'undefined' ? window.g_UsuarioLoginCodigo : 'Invitado';
    const linkDelLogo = window.location.origin + "/images/Logo_Muni.jpg";

    const imgLogo = new Image();
    imgLogo.crossOrigin = "Anonymous";
    imgLogo.src = linkDelLogo;

    imgLogo.onload = function () {
        const ahora = new Date();
        const fechaSistema = ahora.toLocaleDateString("es-PE", { day: "2-digit", month: "2-digit", year: "numeric" });
        const horaSistema = ahora.toLocaleTimeString("es-PE", { hour: "2-digit", minute: "2-digit", second: "2-digit", hour12: true });

        const mesesNombres = ["ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO", "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"];
        const mesTexto = mesesNombres[parseInt(item.mes) - 1] || "MES";
        const periodoTexto = `${mesTexto} - ${item.anio}`;
        const planillaTexto = item.nombrePlanilla ? item.nombrePlanilla.toUpperCase() : "PLANILLA CAS INDETERMINADOS";


        doc.setFont("helvetica", "normal");
        doc.setFontSize(7);
        doc.text(fechaSistema, 143, 7, { align: "right" });
        doc.text(horaSistema, 143, 11, { align: "right" });


        doc.addImage(imgLogo, 'JPEG', 5, 2, 14, 20);


        doc.setFont("helvetica", "bold");
        doc.setFontSize(7);
        doc.text(`BOLETA DE PAGO - ${planillaTexto}`, 148 / 2, 12, { align: "center" });
        doc.setFont("helvetica", "bold");
        doc.setFontSize(6);
        doc.text(`PERIODO: ${periodoTexto}`, 148 / 2, 16, { align: "center" });


        let y = 25;
        doc.setLineWidth(0.15);


        doc.setFont("helvetica", "bold"); doc.setFontSize(7); doc.text("Entidad", 5, y);
        doc.text("Empleador", 5, y + 4);
        doc.text("RUC", 5, y + 8);

        doc.text(":", 20, y); doc.text(":", 20, y + 4); doc.text(":", 20, y + 8);

        doc.setFont("helvetica", "normal"); doc.setFontSize(6);
        doc.text(item.entidad || "Municipalidad Distrital de Ate", 22, y);
        doc.text(item.empleador || "Municipalidad Distrital de Ate", 22, y + 4);
        doc.text(item.ruc || "20131378620", 22, y + 8);


        doc.setFont("helvetica", "bold"); doc.setFontSize(7);
        doc.text("Rubro de Financiamienta", 58, y);
        doc.text("Meta Presupuestal", 58, y + 4);
        doc.text("Unidad Orgánica", 58, y + 8);

        doc.text(":", 90, y); doc.text(":", 90, y + 4); doc.text(":", 90, y + 8);

        doc.setFont("helvetica", "normal"); doc.setFontSize(6);
        doc.text(item.rubro || "-", 92, y);
        doc.text(item.meta || "-", 92, y + 4);

        let centroCosto = item.centroCosto || "-";
        doc.text(centroCosto, 92, y + 8, { maxWidth: 46 });

        y += 11;
        doc.line(5, y, 143, y);
        y += 4;


        doc.setFont("helvetica", "bold"); doc.setFontSize(7); doc.text("Doc. Identidad", 5, y);
        doc.text(":", 38, y);
        doc.setFont("helvetica", "normal"); doc.text(item.dni || "-", 40, y);

        doc.setFont("helvetica", "bold"); doc.text("Código AIRHSP", 74, y);
        doc.text(":", 99, y);
        doc.setFont("helvetica", "normal"); doc.text(item.airhsp || "-", 101, y);

        y += 4;
        doc.setFont("helvetica", "bold"); doc.text("Apellidos y Nombres", 5, y);
        doc.text(":", 38, y);
        doc.setFont("helvetica", "normal"); doc.text(item.nombresCompletos || "-", 40, y, { maxWidth: 98 });

        y += 4;
        doc.setFont("helvetica", "bold"); doc.text("Cargo", 5, y);
        doc.text(":", 38, y);
        doc.setFont("helvetica", "normal");
        let cargoFinal = item.condicionLaboral === "REGIDOR MUNICIPAL" ? "REGIDOR" : (item.cargo || "-");
        doc.text(cargoFinal, 40, y, { maxWidth: 98 });

        y += 4;
        doc.setFont("helvetica", "bold"); doc.text("Fecha de Ingresox", 5, y);
        doc.text(":", 38, y);
        doc.setFont("helvetica", "normal"); doc.text(item.fechaIngreso || "-", 40, y);

        doc.setFont("helvetica", "bold"); doc.text("Establecimiento", 74, y);
        doc.text(":", 99, y);
        doc.setFont("helvetica", "normal"); doc.text(item.sede || "PALACIO MUNICIPAL", 101, y);

        if (item.condicionLaboral !== "REGIDOR MUNICIPAL") {
            y += 4;
            doc.setFont("helvetica", "bold");
            if (planillaTexto !== "PLANILLA PENSIONISTAS") {
                doc.text("Régimen Pensionario", 5, y);
                doc.text("Administrador de Pensión", 5, y + 4);
                doc.text("CUSPP", 5, y + 8);
                doc.text("Fracción y Tipo de Pensión", 5, y + 12);
            } else {
                doc.text("Régimen Laboral", 5, y);
            }

            doc.text(":", 38, y);
            if (planillaTexto !== "PLANILLA PENSIONISTAS") {
                doc.text(":", 38, y + 4); doc.text(":", 38, y + 8); doc.text(":", 38, y + 12);
            }

            doc.setFont("helvetica", "normal");
            if (planillaTexto !== "PLANILLA PENSIONISTAS") {
                doc.text(item.tipoPension || "-", 40, y);
                doc.text(item.adminPens || "-", 40, y + 4);
                doc.text(item.cuspp || "-", 40, y + 8);
                doc.text(item.tipoComision || "-", 40, y + 12);
            } else {
                doc.text(item.condicionLaboral || "-", 40, y);
            }

            doc.setFont("helvetica", "bold");
            doc.text("Régimen Laboral", 74, y);
            doc.text("Condición", 74, y + 4);
            doc.text("Grupo Ocupacional", 74, y + 8);
            doc.text("Cargo Estructural", 74, y + 12);
            doc.text("Jornada Laboral", 74, y + 16);

            doc.text(":", 99, y); doc.text(":", 99, y + 4); doc.text(":", 99, y + 8); doc.text(":", 99, y + 12); doc.text(":", 99, y + 16);

            doc.setFont("helvetica", "normal");
            doc.text(item.condicionLaboral || "-", 101, y);
            doc.text(item.condicion || "-", 101, y + 4);
            doc.text(item.ocupacional || "-", 101, y + 8);
            doc.text(item.estructural || "-", 101, y + 12);
            doc.text(`${item.jornada || 0}`, 101, y + 16);

            y += 20;
        } else {
            y += 4;
        }


        if (item.condicionLaboral !== "REGIDOR MUNICIPAL" && planillaTexto !== "PLANILLA PENSIONISTAS") {
            doc.setFont("helvetica", "bold"); doc.text("Días Laborados", 5, y); doc.text(":", 25, y);
            doc.setFont("helvetica", "normal"); doc.text(`${item.diasLaborados || 0}`, 27, y);

            doc.setFont("helvetica", "bold"); doc.text("Días No Laborados", 45, y); doc.text(":", 69, y);
            doc.setFont("helvetica", "normal"); doc.text(`${item.diasNoLaborados || 0}`, 71, y);

            doc.setFont("helvetica", "bold"); doc.text("Días Subsidiados", 89, y); doc.text(":", 113, y);
            doc.setFont("helvetica", "normal"); doc.text(`${item.subsidios || 0}`, 115, y);

            y += 4;
            doc.setFont("helvetica", "bold"); doc.text("Periodo Vacacional", 5, y); doc.text(":", 30, y);
            doc.setFont("helvetica", "normal"); doc.text(`${item.vacaciones || 0}`, 32, y);
            y += 4;
        }


        doc.line(5, y, 143, y);
        y += 2.2;

        doc.setFont("helvetica", "bold"); doc.setFontSize(7);
        doc.text("CÓDIGO", 12.5, y, { align: "center" });
        doc.text("CONCEPTO", 41.5, y, { align: "center" });
        doc.text("MONTO", 68.5, y, { align: "center" });

        doc.text("CÓDIGO", 81.5, y, { align: "center" });
        doc.text("CONCEPTO", 110.5, y, { align: "center" });
        doc.text("MONTO", 137.5, y, { align: "center" });

        y += 1;
        doc.line(5, y, 143, y);


        const ingresosArr = typeof item.ingresos === 'string' ? JSON.parse(item.ingresos || "[]") : (item.ingresos || []);
        const egresosArr = typeof item.egresos === 'string' ? JSON.parse(item.egresos || "[]") : (item.egresos || []);
        const aportesArr = typeof item.aportes === 'string' ? JSON.parse(item.aportes || "[]") : (item.aportes || []);

        const rowsConceptos = [];


        rowsConceptos.push([
            { content: "INGRESOS", colSpan: 6, styles: { fontStyle: 'bold', fontSize: 7, halign: 'left' } }
        ]);

        const cIng = ingresosArr.length;
        const fIng = Math.ceil(cIng / 2);
        for (let i = 0; i < fIng; i++) {
            const ingCol1 = ingresosArr[i];
            const ingCol2 = ingresosArr[i + fIng];

            rowsConceptos.push([
                ingCol1 ? ingCol1.codigoInterno : "",
                ingCol1 ? ingCol1.concepto : "",
                ingCol1 ? ingCol1.monto.toFixed(2) : "",
                ingCol2 ? ingCol2.codigoInterno : "",
                ingCol2 ? ingCol2.concepto : "",
                ingCol2 ? ingCol2.monto.toFixed(2) : ""
            ]);
        }
        rowsConceptos.push([
            "", "", "", "",
            { content: "TOTAL INGRESOS", styles: { fontStyle: 'bold', halign: 'right', cellPadding: { right: 5 } } },
            { content: item.totalIngresos.toFixed(2), styles: { fontStyle: 'bold', halign: 'right' }, esFilaTotal: true }
        ]);


        rowsConceptos.push([
            { content: "DESCUENTOS", colSpan: 6, styles: { fontStyle: 'bold', fontSize: 7, halign: 'left' }, esTituloBloque: true }
        ]);

        const cEgre = egresosArr.length;
        const fEgre = Math.ceil(cEgre / 2);
        for (let i = 0; i < fEgre; i++) {
            const egrCol1 = egresosArr[i];
            const egrCol2 = egresosArr[i + fEgre];

            rowsConceptos.push([
                egrCol1 ? egrCol1.codigoInterno : "",
                egrCol1 ? egrCol1.concepto : "",
                egrCol1 ? egrCol1.monto.toFixed(2) : "",
                egrCol2 ? egrCol2.codigoInterno : "",
                egrCol2 ? egrCol2.concepto : "",
                egrCol2 ? egrCol2.monto.toFixed(2) : ""
            ]);
        }
        rowsConceptos.push([
            "", "", "", "",
            { content: "TOTAL DESCUENTOS", styles: { fontStyle: 'bold', halign: 'right', cellPadding: { right: 5 } } },
            { content: item.totalEgresos.toFixed(2), styles: { fontStyle: 'bold', halign: 'right' }, esFilaTotal: true }
        ]);


        if (aportesArr.length > 0) {
            rowsConceptos.push([
                { content: "APORTES", colSpan: 6, styles: { fontStyle: 'bold', fontSize: 7, halign: 'left' }, esTituloBloque: true }
            ]);

            const cApor = aportesArr.length;
            const fApor = Math.ceil(cApor / 2);
            let totalAportado = aportesArr.reduce((acc, a) => acc + a.monto, 0);

            for (let i = 0; i < fApor; i++) {
                const apoCol1 = aportesArr[i];
                const apoCol2 = aportesArr[i + fApor];

                rowsConceptos.push([
                    apoCol1 ? apoCol1.codigoInterno : "",
                    apoCol1 ? apoCol1.concepto : "",
                    apoCol1 ? apoCol1.monto.toFixed(2) : "",
                    apoCol2 ? apoCol2.codigoInterno : "",
                    apoCol2 ? apoCol2.concepto : "",
                    apoCol2 ? apoCol2.monto.toFixed(2) : ""
                ]);
            }
            rowsConceptos.push([
                "", "", "", "",
                { content: "TOTAL APORTES", styles: { fontStyle: 'bold', halign: 'right', cellPadding: { right: 5 } } },
                { content: totalAportado.toFixed(2), styles: { fontStyle: 'bold', halign: 'right' }, esFilaTotal: true }
            ]);
        }


        doc.autoTable({
            startY: y + 1.2,
            margin: { left: 5, right: 5 },
            theme: 'plain',
            showHead: 'never',
            body: rowsConceptos,
            styles: { font: "helvetica", fontSize: 6, textColor: [0, 0, 0], cellPadding: 0.25 },
            columnStyles: {
                0: { width: 15, halign: 'center' },
                1: { width: 39, halign: 'left' },
                2: { width: 15, halign: 'right' },
                3: { width: 15, cellPadding: { left: 2 }, halign: 'center' },
                4: { width: 39, halign: 'left' },
                5: { width: 15, halign: 'right' }
            },
            didDrawCell: function (data) {
                const celdaDatos = data.row.raw[data.column.index];


                if (celdaDatos && celdaDatos.esFilaTotal && data.column.index === 5) {
                    doc.setLineWidth(0.15);
                    doc.setDrawColor(0, 0, 0);


                    let xFin = data.cell.x + data.cell.width;

                    let xInicio = data.cell.x - 5;

                    doc.line(xInicio, data.cell.y, xFin, data.cell.y);
                }


                const celdaCero = data.row.raw[0];
                if (celdaCero && celdaCero.esTituloBloque && data.column.index === 0) {
                    doc.setLineWidth(0.15);
                    doc.setDrawColor(0, 0, 0);
                    let yLineaBajoTitulo = data.cell.y + data.cell.height;
                    doc.line(5, yLineaBajoTitulo, 143, yLineaBajoTitulo);
                }
            }
        });


        let currentY = doc.lastAutoTable.finalY + 4;


        let inicioLineaX = pageWidth / 2;

        doc.setLineWidth(0.15);
        doc.line(inicioLineaX, currentY, 143, currentY);

        currentY += 4;
        doc.setFont("helvetica", "bold"); doc.setFontSize(7);
        doc.text("NETO A PAGAR", 89, currentY);

        let netoCalculado = item.netoPagar;
        if (Math.abs(netoCalculado) < 0.01) {
            netoCalculado = 0.00;
        }
        doc.text(netoCalculado.toFixed(2), 143, currentY, { align: "right" });

        currentY += 2;
        doc.line(inicioLineaX, currentY, 143, currentY);


        doc.setFont("helvetica", "normal"); doc.setFontSize(6);


        doc.line(17, 185, 17 + 40, 185);
        doc.text('Firma del Trabajador', 17 + 20, 189, { align: 'center' });


        doc.line(91, 185, 91 + 40, 185);
        doc.text('Firma de RRHH', 91 + 20, 189, { align: 'center' });

        const dniLimpio = (item.dni || "SIN_DNI").trim();
        const nombreDescarga = `${dniLimpio}_${mesTexto}_${item.anio}`;

        doc.setProperties({
            title: nombreDescarga
        });

        const blobUrl = doc.output("bloburl");
        window.open(blobUrl, "_blank");
    };

    imgLogo.onerror = function () {
        alert("No se pudo cargar el logo oficial, abriendo visor de boleta sin imagen.");
        imgLogo.onload();
    };
}





