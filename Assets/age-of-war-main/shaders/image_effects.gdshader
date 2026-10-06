shader_type canvas_item;

uniform vec2 mouse_pos = vec2(0.5, 0.5);
uniform vec2 card_size = vec2(200.0, 300.0);
uniform float tilt_strength = 0.3;
uniform float perspective_depth = 1000.0;

// Glint config
uniform float shimmer_width = 0.15;
uniform float shimmer_strength = 0.4;
uniform vec3 shimmer_color = vec3(1.0, 1.0, 1.0); // white shimmer
uniform float shimmer_softness = 0.01;

void vertex() {
    vec2 center = card_size * 0.5;
    vec2 local = VERTEX.xy - center;

    vec2 mouse = (mouse_pos * 2.0 - 1.0);

    float rot_x = -mouse.y * tilt_strength;
    float rot_y = mouse.x * tilt_strength;

    float z = 0.0;
    float cosx = cos(rot_x);
    float sinx = sin(rot_x);
    float cosy = cos(rot_y);
    float siny = sin(rot_y);

    float x = local.x * cosy + z * siny;
    z = -local.x * siny + z * cosy;

    float y = local.y * cosx - z * sinx;
    z = local.y * sinx + z * cosx;

    float factor = perspective_depth / (perspective_depth + z);
    vec2 projected = vec2(x, y) * factor;

    VERTEX.xy = projected + center;
}

void fragment() {
    vec4 base = texture(TEXTURE, UV);

    // Compute direction of shimmer based on mouse tilt
    vec2 dir = normalize(mouse_pos * 2.0 - 1.0);

    // Diagonal shimmer line equation (dot product line sweep)
    float line = dot(UV - vec2(0.5), normalize(vec2(-dir.y, dir.x))); // perpendicular to tilt

    // Create shimmer band with smooth edges
    float shine = exp(-pow(line / shimmer_width, 2.0)) * shimmer_strength;

    // Optional softness fade toward edges
    float fade = smoothstep(0.5, 0.0, length(UV - vec2(0.5)));
    shine *= fade;

    base.rgb += shine * shimmer_color;

    COLOR = base;
}
