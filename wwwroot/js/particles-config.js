particlesJS("particles-js", {
    particles: {
        number: { value: 120, density: { enable: true, value_area: 1000 } }, // Số hạt và mật độ
        color: { value: ["#60a5fa", "#3b82f6", "#2563eb"] }, // Gradient màu xanh nhạt đến xanh đậm
        shape: { type: "circle", stroke: { width: 0, color: "#000000" } }, // Hình dạng chấm
        opacity: { value: 0.9, random: false, anim: { enable: false } }, // Độ mờ cố định, tắt nhấp nháy
        size: { value: 3, random: true, anim: { enable: false } }, // Kích thước cố định, tắt nhấp nháy
        line_linked: { enable: true, distance: 100, color: "#60a5fa", opacity: 0.4, width: 1 }, // Mạng lưới với đường nối xanh nhạt
        move: {
            enable: true,
            speed: 3,
            direction: "none",
            random: true,
            straight: false,
            out_mode: "out",
            bounce: false,
            attract: { enable: true, rotateX: 600, rotateY: 1200 } // Hiệu ứng xoáy nhẹ
        }
    },
    interactivity: {
        detect_on: "canvas",
        events: {
            onhover: { enable: true, mode: ["grab"] }, // Kéo chấm lại gần khi hover
            onclick: { enable: true, mode: "push" }, // Thêm chấm khi click
            resize: true
        },
        modes: {
            grab: { distance: 120, line_linked: { opacity: 0.7 } }, // Đường nối sáng hơn khi hover
            push: { particles_nb: 3 } // Thêm 3 chấm khi click
        }
    },
    retina_detect: true
});

// Hàm điều chỉnh chiều cao particles theo trang
function updateParticlesHeight() {
    const particlesContainer = document.getElementById('particles-js');
    if (particlesContainer) {
        const bodyHeight = document.body.scrollHeight;
        particlesContainer.style.height = `${bodyHeight}px`;
        if (window.pJSDom && window.pJSDom[0] && window.pJSDom[0].pJS) {
            window.pJSDom[0].pJS.fn.vendors.resize();
        }
    }
}

// Chạy khi trang tải, resize hoặc scroll
window.addEventListener('load', updateParticlesHeight);
window.addEventListener('resize', updateParticlesHeight);
window.addEventListener('scroll', updateParticlesHeight);