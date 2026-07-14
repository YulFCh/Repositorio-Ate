let boletaSeleccionadaGlobal = null;

const txtDni = document.getElementById("dni");
const txtCorreo = document.getElementById("correo");
const txtCodigo = document.getElementById("codigo_verificacion");
const btnEnviarCodigo = document.getElementById("btnEnviarCodigo");


const API_IP = "http://192.168.4.91:203";

////
document.addEventListener("DOMContentLoaded", function () {

    document.getElementById("searchForm").addEventListener("submit", async function (e) {
        e.preventDefault();

        const tokenSeguridad = sessionStorage.getItem("consulta_token");

        if (!tokenSeguridad) {
            alert("No se encontró una sesión activa. Por favor, valide el código primero.");
            return;
        }

        const anio = document.getElementById("anio").value;
        const mesDesde = document.getElementById("mesDesde").value;
        const mesHasta = document.getElementById("mesHasta").value;

        const dniClean = txtDni.value.trim();
        let url = `${API_IP}/api/ConsultaDni/historial/${dniClean}`;

        const params = [];
        if (anio) params.push(`anio=${anio}`);
        if (mesDesde) params.push(`mesDesde=${mesDesde}`);
        if (mesHasta) params.push(`mesHasta=${mesHasta}`);

        if (params.length > 0) {
            url += `?${params.join("&")}`;
        }

        try {
            document.getElementById("loadingSpinner").classList.remove("d-none");
            document.getElementById("searchIcon").classList.add("d-none");

            const response = await fetch(url, {
                method: "GET",
                headers: {
                    "Accept": "application/json",
                    "X-Consulta-Token": tokenSeguridad
                }
            });

            if (response.status === 401 || response.status === 403) {
                alert(`Acceso denegado (${response.status}). El token no es válido para consultar este DNI en el Backend.`);
                return;
            }

            if (!response.ok) throw new Error("Error al consultar la API");

            const data = await response.json();
            cargarTabla(data);

        } catch (error) {
            alert("Ocurrió un error al cargar los registros. Inténtelo más tarde.");
        } finally {
            document.getElementById("loadingSpinner").classList.add("d-none");
            document.getElementById("searchIcon").classList.remove("d-none");
        }
    });


    const checkAll = document.getElementById("checkAllBoletas");
    if (checkAll) {
        checkAll.addEventListener("change", function () {
            const checkboxes = document.querySelectorAll(".chk-boleta");
            checkboxes.forEach(chk => chk.checked = this.checked);
            actualizarBotonesSeleccionados();
        });
    }


    const boletasTable = document.querySelector("#boletasTable tbody");
    if (boletasTable) {
        boletasTable.addEventListener("change", function (e) {
            if (e.target.classList.contains("chk-boleta")) {
                const totalInputs = document.querySelectorAll(".chk-boleta").length;
                const totalChecked = document.querySelectorAll(".chk-boleta:checked").length;

                const masterCheck = document.getElementById("checkAllBoletas");
                if (masterCheck) masterCheck.checked = (totalInputs === totalChecked);
                actualizarBotonesSeleccionados();
            }
        });
    }

    const btnVerSeleccionados = document.getElementById("btnVerSeleccionados");
    if (btnVerSeleccionados) {
        btnVerSeleccionados.addEventListener("click", verBoletasSeleccionadas);
    }
});



function cargarTabla(lista) {
    const tbody = document.querySelector("#boletasTable tbody");
    tbody.innerHTML = "";


    const checkAll = document.getElementById("checkAllBoletas");
    if (checkAll) checkAll.checked = false;
    actualizarBotonesSeleccionados();

    if (lista.length === 0) {
        document.getElementById("resultsCard").classList.add("d-none");
        return;
    }

    lista.forEach((item, index) => {
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
                    <td class="text-center ps-4">
                        <input class="form-check-input shadow-sm chk-boleta" type="checkbox" data-index="${index}" value='${itemString}' style="cursor: pointer;">
                    </td>
                    <td class="fw-semibold text-dark">${periodo}</td>
                    <td>${item.dni}</td>
                    <td><span class="badge bg-light text-secondary border px-2 py-1">${codEmp}</span></td>
                    <td class="fw-medium text-dark">${item.nombresCompletos}</td>
                    <td>${item.condicionLaboral}</td>
                    <td>${item.cargo}</td>
                    <td>${item.centroCosto}</td>
                    <td>${item.fechaIngreso}</td>
                    <td>${regimenDesc}</td>
                    <td class="text-end">
                        <span class="badge ${badgeColor} px-3 py-2 rounded-pill fw-semibold small">${situacion}</span>
                    </td>
                    
                </tr>`;
    });

    document.getElementById("resultsCard").classList.remove("d-none");
}

function actualizarBotonesSeleccionados() {
    const seleccionados = document.querySelectorAll(".chk-boleta:checked").length;
    const btnMasivo = document.getElementById("btnVerSeleccionados");
    const lblCant = document.getElementById("lblCantSeleccionados");

    if (lblCant) lblCant.innerText = seleccionados;

    if (btnMasivo) {
        if (seleccionados > 0) {
            btnMasivo.removeAttribute("disabled");
            btnMasivo.classList.replace("btn-secondary", "btn-primary");
        } else {
            btnMasivo.setAttribute("disabled", "true");
            btnMasivo.classList.replace("btn-primary", "btn-secondary");
        }
    }
}


function imprimirBoletasSeleccionadasPDF() {
    const checkboxes = document.querySelectorAll(".chk-boleta:checked");
    if (checkboxes.length === 0) {
        alert("Por favor, seleccione al menos una boleta para generar el PDF.");
        return;
    }

    let jsPDFWindow = window.jspdf ? window.jspdf.jsPDF : window.jsPDF;
    if (!jsPDFWindow) {
        alert("La librería jsPDF no está cargada correctamente. Verifica tus scripts.");
        return;
    }

    const doc = new jsPDFWindow({ orientation: 'portrait', unit: 'mm', format: 'a5' });
    const pageWidth = doc.internal.pageSize.width;

    const linkDelLogo = window.location.origin + "/images/Logo_Muni.jpg";
    const imgLogo = new Image();
    imgLogo.crossOrigin = "Anonymous";
    imgLogo.src = linkDelLogo;

    imgLogo.onload = function () {
        checkboxes.forEach((chk, index) => {
            const item = JSON.parse(chk.value.replace(/&@@apos;/g, "'"));

            if (index > 0) {
                doc.addPage({ orientation: 'portrait', format: 'a5' });
            }
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
            doc.text("Rubro de Financiamiento", 58, y);
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
            doc.setFont("helvetica", "bold"); doc.text("Fecha de Ingreso", 5, y);
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
            rowsConceptos.push([{ content: "INGRESOS", colSpan: 6, styles: { fontStyle: 'bold', fontSize: 7, halign: 'left' } }]);

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
            rowsConceptos.push(["", "", "", "", { content: "TOTAL INGRESOS", styles: { fontStyle: 'bold', halign: 'right', cellPadding: { right: 5 } } }, { content: item.totalIngresos.toFixed(2), styles: { fontStyle: 'bold', halign: 'right' }, esFilaTotal: true }]);

            rowsConceptos.push([{ content: "DESCUENTOS", colSpan: 6, styles: { fontStyle: 'bold', fontSize: 7, halign: 'left' }, esTituloBloque: true }]);
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
            rowsConceptos.push(["", "", "", "", { content: "TOTAL DESCUENTOS", styles: { fontStyle: 'bold', halign: 'right', cellPadding: { right: 5 } } }, { content: item.totalEgresos.toFixed(2), styles: { fontStyle: 'bold', halign: 'right' }, esFilaTotal: true }]);

            if (aportesArr.length > 0) {
                rowsConceptos.push([{ content: "APORTES", colSpan: 6, styles: { fontStyle: 'bold', fontSize: 7, halign: 'left' }, esTituloBloque: true }]);
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
                rowsConceptos.push(["", "", "", "", { content: "TOTAL APORTES", styles: { fontStyle: 'bold', halign: 'right', cellPadding: { right: 5 } } }, { content: totalAportado.toFixed(2), styles: { fontStyle: 'bold', halign: 'right' }, esFilaTotal: true }]);
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
                        doc.setLineWidth(0.15); doc.setDrawColor(0, 0, 0);
                        doc.line(data.cell.x - 5, data.cell.y, data.cell.x + data.cell.width, data.cell.y);
                    }
                    const celdaCero = data.row.raw[0];
                    if (celdaCero && celdaCero.esTituloBloque && data.column.index === 0) {
                        doc.setLineWidth(0.15); doc.setDrawColor(0, 0, 0);
                        doc.line(5, data.cell.y + data.cell.height, 143, data.cell.y + data.cell.height);
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
            if (Math.abs(netoCalculado) < 0.01) netoCalculado = 0.00;
            doc.text(netoCalculado.toFixed(2), 143, currentY, { align: "right" });

            currentY += 2;
            doc.line(inicioLineaX, currentY, 143, currentY);

            doc.setFont("helvetica", "normal"); doc.setFontSize(6);
            doc.line(17, 185, 17 + 40, 185);
            doc.text('Firma del Trabajador', 17 + 20, 189, { align: 'center' });

            doc.line(91, 185, 91 + 40, 185);
            doc.text('Firma de RRHH', 91 + 20, 189, { align: 'center' });

        });



        let dniArchivo = "";
        let anioArchivo = "";
        let mesesSeleccionados = [];

        checkboxes.forEach(chk => {

            const item = JSON.parse(chk.value.replace(/&@@apos;/g, "'"));

            if (!dniArchivo) {
                dniArchivo = item.dni || "";
                anioArchivo = item.anio || "";
            }

            mesesSeleccionados.push(parseInt(item.mes));
        });


        const mesesNombresArchivo = [
            "ENERO", "FEBRERO", "MARZO", "ABRIL",
            "MAYO", "JUNIO", "JULIO", "AGOSTO",
            "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
        ];


        const mesInicio = Math.min(...mesesSeleccionados);
        const mesFinal = Math.max(...mesesSeleccionados);


        let rangoMes = mesesNombresArchivo[mesInicio - 1];

        if (mesInicio !== mesFinal) {
            rangoMes += " A " + mesesNombresArchivo[mesFinal - 1];
        }


        const nombreArchivo = `BOLETA_${dniArchivo}_${rangoMes}_${anioArchivo}.pdf`;

        doc.setProperties({
            title: nombreArchivo.replace(".pdf", "")
        });

        const blobUrl = doc.output("bloburl");
        window.open(blobUrl, "_blank");
    };

    imgLogo.onerror = function () {
        alert("No se pudo cargar el logo oficial, generando el lote sin imagen.");
        imgLogo.onload();
    };
}



document.getElementById("btnVerSeleccionados").addEventListener("click", function () {

    const checkboxes = document.querySelectorAll(".chk-boleta:checked");

    if (checkboxes.length === 0) {
        alert("Por favor, seleccione al menos una boleta.");
        return;
    }


    const listaBoletas = Array.from(checkboxes).map(chk => {
        const rawJson = chk.value.replace(/&@@apos;/g, "'");
        const objetoParseado = JSON.parse(rawJson);

        return objetoParseado;
    });


    mostrarMultiplesBoletasEnModal(listaBoletas);
});

function mostrarMultiplesBoletasEnModal(listaBoletas) {

    if (!listaBoletas || listaBoletas.length === 0) {
        return;
    }

    const plantilla = document.getElementById("boletaImprimible");
    const modalBody = document.querySelector("#boletaModal .modal-body");

    if (!window.plantillaModalBoleta) {
        window.plantillaModalBoleta = plantilla.outerHTML;
    }


    modalBody.innerHTML = "";

    listaBoletas.forEach((item, index) => {

        const contenedor = document.createElement("div");
        contenedor.innerHTML = window.plantillaModalBoleta;

        const mesesNombres = [
            "ENERO", "FEBRERO", "MARZO", "ABRIL",
            "MAYO", "JUNIO", "JULIO", "AGOSTO",
            "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
        ];

        const mesTexto = mesesNombres[parseInt(item.mes) - 1] || "MES";

        const asignarTexto = (selector, valor) => {
            const el = contenedor.querySelector(selector);
            if (el) {
                el.innerText = valor ?? "";
            }
        };


        asignarTexto("#lblPeriodoTitulo", `${mesTexto} - ${item.anio}`);
        asignarTexto("#lblTituloPlanilla", item.nombrePlanilla ? item.nombrePlanilla.toUpperCase() : "PLANILLA");
        asignarTexto("#lblEntidad", item.entidad || "Municipalidad Distrital de Ate");
        asignarTexto("#lblEmpleador", item.empleador || "Municipalidad Distrital de Ate");
        asignarTexto("#lblRuc", item.ruc || "20131378620");
        asignarTexto("#lblRubro", item.rubro || "-");
        asignarTexto("#lblMeta", item.meta || "-");
        asignarTexto("#lblUnidadOrganica", item.centroCosto || "-");

        asignarTexto("#lblDni", item.dni);
        asignarTexto("#lblAirhsp", item.airhsp || "-");
        asignarTexto("#lblNombre", item.nombresCompletos);
        asignarTexto("#lblCargo", item.cargo);
        asignarTexto("#lblFechaIngreso", item.fechaIngreso);

        asignarTexto("#lblRegimen", item.tipoPension || "-");
        asignarTexto("#lblAdminPens", item.adminPens || "-");
        asignarTexto("#lblCuspp", item.cuspp || "-");

        asignarTexto("#lblSede", item.sede || "PALACIO MUNICIPAL");
        asignarTexto("#lblRegimenLaboral", item.condicionLaboral || "-");
        asignarTexto("#lblCondicion", item.condicion || "-");
        asignarTexto("#lblOcupacional", item.ocupacional || "-");
        asignarTexto("#lblEstructural", item.estructural || "-");
        asignarTexto("#lblCodEmpleado", item.codEmpleado || item.idEmpleado);
        asignarTexto("#lblTipoComision", item.tipoComision || "-");

        asignarTexto("#lblJornada", item.jornada || 0);
        asignarTexto("#lblDiasLab", item.diasLaborados || 0);
        asignarTexto("#lblDiasNoLab", item.diasNoLaborados || 0);
        asignarTexto("#lblSubsidios", item.subsidios || 0);
        asignarTexto("#lblVacaciones", item.vacaciones || 0);

        asignarTexto("#lblTotalIngresos", Number(item.totalIngresos || 0).toFixed(2));
        asignarTexto("#lblTotalEgresos", Number(item.totalEgresos || 0).toFixed(2));
        asignarTexto("#lblNetoPagar", Number(item.netoPagar || 0).toFixed(2));


        const ingresos = typeof item.ingresos === "string"
            ? JSON.parse(item.ingresos || "[]")
            : (item.ingresos || []);

        let htmlIngresos = "";

        ingresos.forEach(i => {
            htmlIngresos += `
                <tr>
                    <td>${i.codigoInterno} - ${i.concepto}</td>
                    <td class="text-end text-success">+ S/ ${Number(i.monto).toFixed(2)}</td>
                </tr>`;
        });

        const tblIngresos = contenedor.querySelector("#tblModalIngresos tbody");
        if (tblIngresos) {
            tblIngresos.innerHTML = htmlIngresos || "<tr><td colspan='2' class='text-center'>Sin registros</td></tr>";
        }



        const egresos = typeof item.egresos === "string"
            ? JSON.parse(item.egresos || "[]")
            : (item.egresos || []);

        let htmlEgresos = "";

        egresos.forEach(i => {
            htmlEgresos += `
                <tr>
                    <td>${i.codigoInterno} - ${i.concepto}</td>
                    <td class="text-end text-danger">- S/ ${Number(i.monto).toFixed(2)}</td>
                </tr>`;
        });

        const tblEgresos = contenedor.querySelector("#tblModalEgresos tbody");
        if (tblEgresos) {
            tblEgresos.innerHTML = htmlEgresos || "<tr><td colspan='2' class='text-center'>Sin registros</td></tr>";
        }

  

        const aportes = typeof item.aportes === "string"
            ? JSON.parse(item.aportes || "[]")
            : (item.aportes || []);

        let htmlAportes = "";
        let totalAportes = 0;

        aportes.forEach(i => {

            totalAportes += Number(i.monto);

            htmlAportes += `
                <tr>
                    <td>${i.codigoInterno} - ${i.concepto}</td>
                    <td class="text-end text-primary">S/ ${Number(i.monto).toFixed(2)}</td>
                </tr>`;
        });

        const tblAportes = contenedor.querySelector("#tblModalAportes tbody");
        if (tblAportes) {
            tblAportes.innerHTML = htmlAportes || "<tr><td colspan='2' class='text-center'>Sin registros</td></tr>";
        }

        asignarTexto("#lblTotalAportes", totalAportes.toFixed(2));

        contenedor.querySelectorAll("[id]").forEach(el => el.removeAttribute("id"));

        modalBody.appendChild(contenedor.firstElementChild);

        if (index < listaBoletas.length - 1) {
            modalBody.insertAdjacentHTML(
                "beforeend",
                "<hr class='my-5 border-2 border-primary-subtle'>"
            );
        }
    });

    bootstrap.Modal.getOrCreateInstance(
        document.getElementById("boletaModal")
    ).show();
}


/////////////////////////



document.addEventListener("DOMContentLoaded", function () {
    const txtDni = document.getElementById("dni");
    const txtCorreo = document.getElementById("correo");
    const txtCodigo = document.getElementById("codigo_verificacion");
    const btnEnviarCodigo = document.getElementById("btnEnviarCodigo");

    if (txtDni) {
        txtDni.addEventListener("input", function () {
            const dni = txtDni.value.trim();

            if (dni.length >= 8) {
                txtCorreo.disabled = false;
            } else {
                txtCorreo.disabled = true;
                txtCorreo.value = "";
                btnEnviarCodigo.disabled = true;

                const seccion = document.getElementById("seccionFiltrosConsulta");
                if (seccion) {
                    seccion.classList.add("d-none");
                }
            }
        });
    }

    if (txtCorreo) {
        txtCorreo.addEventListener("input", function () {
            const correo = txtCorreo.value.trim();

            if (correo.length > 5) {
                btnEnviarCodigo.disabled = false;
            } else {
                btnEnviarCodigo.disabled = true;
            }
        });
    }


    if (btnEnviarCodigo) {
        btnEnviarCodigo.addEventListener("click", async function () {
            const dni = txtDni.value.trim();
            const correo = txtCorreo.value.trim();

            if (!dni || !correo) {
                alert("Ingrese DNI y correo.");
                return;
            }

            try {
                btnEnviarCodigo.disabled = true; 

                const response = await fetch(`${API_IP}/api/ConsultaDni/enviar-codigo/${dni}`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json"
                    },
                    body: JSON.stringify(correo)
                });

                const data = await response.json();
               // console.log("Respuesta enviar código:", data);

                if (!response.ok) {
                    alert(data.mensaje || "Error al enviar el código.");
                    btnEnviarCodigo.disabled = false;
                    return;
                }

                alert("Código enviado correctamente. Revise su correo.");

                txtCodigo.disabled = false;
                txtCodigo.focus();

            } catch (error) {
                //console.error("Error completo al enviar código:", error);
                alert("Error al procesar el envío: " + error.message);
                btnEnviarCodigo.disabled = false;
            }
        });
    }

    if (txtCodigo) {
        txtCodigo.addEventListener("input", async function () {
            const codigoClean = txtCodigo.value.trim();
            const dniClean = txtDni.value.trim();

            if (codigoClean.length !== 6) return;

            try {
                txtCodigo.disabled = true;

                const urlValidar = `${API_IP}/api/ConsultaDni/validar-codigo`;
               // console.log("Validando código en:", urlValidar, { dni: dniClean, codigo: codigoClean });

                const response = await fetch(urlValidar, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Accept": "application/json"
                    },
                    body: JSON.stringify({
                        dni: dniClean,
                        codigo: codigoClean
                    })
                });

                const texto = await response.text();

                if (!response.ok) {
                    alert(texto || "Código incorrecto o expirado.");
                    txtCodigo.value = "";
                    txtCodigo.disabled = false;
                    return;
                }

                const data = JSON.parse(texto);

                

                sessionStorage.setItem("consulta_token", data.token);

                alert(data.mensaje || "Código correcto. Consulta habilitada.");


                const seccionFiltrosConsulta = document.getElementById("seccionFiltrosConsulta");
                if (seccionFiltrosConsulta) {
                    seccionFiltrosConsulta.classList.remove("d-none");
                }

                txtDni.readOnly = true;
                txtCorreo.readOnly = true;
                txtCodigo.readOnly = true;
                btnEnviarCodigo.disabled = true;

            } catch (error) {
                //console.error("Error crítico en la validación:", error);
                alert("Ocurrió un problema al validar el código.");
                txtCodigo.disabled = false;
            }
        });
    }
});